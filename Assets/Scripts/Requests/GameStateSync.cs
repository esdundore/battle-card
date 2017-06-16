using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameStateSync : MonoBehaviour {

    public static GameStateSync Instance;

    // GLOBAL SETTINGS
    [Header("Colors")]
    public Color32 CardBodyStandardColor;
    public Color32 CardRibbonsStandardColor;
    public Color32 CardGlowColor;
    [Header("Numbers and Values")]
    public float CardPreviewTime = 1f;
    public float CardTransitionTime = 1f;
    public float CardPreviewTimeFast = 1f;
    public float CardTransitionTimeFast = 1f;
    public float MonsterFlipTime = 1f;
    [Header("Prefabs and Assets")]
    public GameObject SkillCardPrefab;
    public GameObject ArrowTarget;
    public GameObject MonsterCardPrefab;
    public GameObject AvatarPrefab;
    public GameObject DamageEffectPrefab;

    [Header("Game State")]
    // visual player areas
    public PlayerArea playerArea;
    public PlayerArea opponentArea;

    // the game view
    public GameView gameView = new GameView();
    public PlayableCardView playableCardView = new PlayableCardView();

    public PlayersRequest playersRequest = new PlayersRequest();
    public PlayableRequest playableRequest = new PlayableRequest();
    public GutsRequest gutsRequest = new GutsRequest();
    public DefendRequest defendRequest = new DefendRequest();

    public static string PLAYER1 = "player1";
    public static string PLAYER2 = "player2";
    public static string GUTS_PHASE = "guts";
    public static string ATTACK_PHASE = "attack";
    public static string DEFEND_PHASE = "defend";

    private static string SERVER_NAME = "http://localhost:8080";
    private static string START_REQUEST = "/start-match";
    private static string SYNC_REQUEST = "/get-game";
    private static string PLAYABLE_REQUEST = "/playable-cards";
    private static string GUTS_REQUEST = "/make-guts";
    private static string DEFEND_REQUEST = "/defend";
    private static string END_ATTACK_REQUEST = "/end-attack";

    private float MESSAGE_TIME = 1.5f;

    // Singleton
    private void Awake()
    {
        Instance = this;
    }

    // On object creation
    void Start()
    {
        playersRequest.player1 = PLAYER1;
        playersRequest.player2 = PLAYER2;
        playableRequest.player1 = PLAYER1;
        playableRequest.player2 = PLAYER2;
        playableRequest.playedCardIndexes = new List<int>();
        gutsRequest.player1 = PLAYER1;
        gutsRequest.player2 = PLAYER2;
        gutsRequest.discards = new List<int>();
        defendRequest.player1 = PLAYER1;
        defendRequest.player2 = PLAYER2;
        defendRequest.cardAndTargets = new List<DefendTarget>();
        playerArea.button.GetComponent<Button>().interactable = false;
        StartCoroutine(GameFlow());
    }

    // Game flow including start, syncing visuals, and finish
    public IEnumerator GameFlow()
    {

        // Start the match
        CoroutineWithData cd1 = new CoroutineWithData(this, RESTTemplate.SyncPOST(SERVER_NAME + START_REQUEST, JsonUtility.ToJson(playersRequest)));
        yield return cd1.coroutine;

        // Start syncing the match
        while (true)
        {
            CoroutineWithData cd2 = new CoroutineWithData(this, GameSync());
            yield return cd2.coroutine;
        }

        // Finish the match?

    }

    // Sync game view with the server
    public IEnumerator GameSync()
    {
        // Get the new game view
        CoroutineWithData cd = new CoroutineWithData(this, RESTTemplate.SyncPOST(SERVER_NAME + SYNC_REQUEST, JsonUtility.ToJson(playersRequest)));
        yield return cd.coroutine;
        string resultBody = (string) cd.result;
        GameView newGameView = JsonUtility.FromJson<GameView>(resultBody);
        Debug.Log(resultBody);

        // play monsters
        PlayMonsters(opponentArea, ref gameView.opponent.monsters, newGameView.opponent.monsters);
        PlayMonsters(playerArea, ref gameView.player.monsters, newGameView.player.monsters);

        // adjust guts
        SyncGuts(opponentArea, ref gameView.opponent.gutsPool, newGameView.opponent.gutsPool);
        SyncGuts(playerArea, ref gameView.player.gutsPool, newGameView.player.gutsPool);

        // draw
        SyncHand(opponentArea, ref gameView.opponent.hand, newGameView.opponent.hand, false);
        SyncHand(playerArea, ref gameView.player.hand, newGameView.player.hand, true);

        // adjust deck size
        SyncDeck(opponentArea, ref gameView.opponent.deckSize, newGameView.opponent.deckSize);
        SyncDeck(playerArea, ref gameView.player.deckSize, newGameView.player.deckSize);

        // play opponent attack request or defend request
        SyncTable(opponentArea, playerArea, ref gameView.attackView, newGameView.attackView, ref gameView.defendView, newGameView.defendView);

        // sync monster state
        SyncMonsters(opponentArea, ref gameView.opponent.monsters, newGameView.opponent.monsters);
        SyncMonsters(playerArea, ref gameView.player.monsters, newGameView.player.monsters);

        // show game phase
        SyncPlayerAndPhase(playerArea, ref gameView.currentPlayer, newGameView.currentPlayer, ref gameView.phase, newGameView.phase);

        yield return new WaitForSeconds(1);
    }

    // Play monsters
    public void PlayMonsters(PlayerArea area, ref List<Monster> playerMonsters, List<Monster> newPlayerMonsters)
    {
        if (playerMonsters.Count == 0)
        {
            playerMonsters = new List<Monster>();

            for (int i = 0; i < newPlayerMonsters.Count; i++)
            {
                // play this monster for the first time
                Monster monster = newPlayerMonsters[i];
                MonsterAsset monsterAsset = Resources.Load<MonsterAsset>("Cards/" + monster.id + "/" + monster.id) as MonsterAsset;
                new PlayAMonsterCommand(area, monsterAsset, i).AddToQueue();
                // sync monster data to local
                playerMonsters.Insert(i, monster);
            }
        }
    }

    // Draw or discard cards from hand
    public void SyncHand(PlayerArea area, ref List<string> playerHand, List<string> newPlayerHand, bool canSee)
    {
        for (int i = 0; i < newPlayerHand.Count; i++)
        {
            if (i > playerHand.Count - 1)
            {
                playerHand.Add(null);
            }
            if (!newPlayerHand[i].Equals(playerHand[i]))
            {
                // remove if current card is not null
                if (playerHand[i] != null && !"".Equals(playerHand[i]))
                {
                    Debug.Log("removing a card");
                    new RemoveACardCommand(area, i, true).AddToQueue();
                }
                // draw if new card is not null
                if (newPlayerHand[i] != null && !"".Equals(newPlayerHand[i]))
                {
                    new DrawACardCommand(area, FindCardAsset(newPlayerHand[i]), i, canSee).AddToQueue();
                }
                // sync hand data to local
                playerHand[i] = newPlayerHand[i];
            }
        }
    }

    // Sync deck card count and size
    public void SyncDeck(PlayerArea area, ref int deckSize, int newDeckSize)
    {
        if (deckSize != newDeckSize)
        {
            new ChangeTextCommand(ref area.deckVisual.DeckSizeText, newDeckSize.ToString()).AddToQueue();
            deckSize = newDeckSize;
        }
    }

    // Sync guts amount
    public void SyncGuts(PlayerArea area, ref int gutsAmount, int newGutsAmount)
    {
        if (gutsAmount != newGutsAmount)
        {
            new ChangeTextCommand(ref area.avatar.gutsText, newGutsAmount.ToString()).AddToQueue();
            gutsAmount = newGutsAmount;
        }
    }

    // TODO: Play currently attack or defense cards from the server
    public void SyncTable(PlayerArea area, PlayerArea targetArea, ref AttackView attackView, AttackView newAttackView, ref DefendView defendView, DefendView newDefendView)
    {

        if (!newAttackView.cardsPlayed.SequenceEqual(attackView.cardsPlayed) && newAttackView.cardsPlayed != null && newAttackView.player1.Equals(PLAYER2))
        {
            // play skill card to table
            for (int i = 0; i < newAttackView.cardsPlayed.Count; i++)
            {
                new RemoveACardCommand(area, newAttackView.handIndexes[i], true).AddToQueue();
                new PlaySkillCardCommand(area, FindCardAsset(newAttackView.cardsPlayed[i]), newAttackView.handIndexes[i]).AddToQueue();
            }
            // highlight attacking monster
            new HighlightCommand(area, newAttackView.user, true).AddToQueue();
            // draw targeting arrows
            for (int i = 0; i < newAttackView.targets.Count; i++)
            {
                new DrawArrowCommand(area, targetArea, newAttackView.user, newAttackView.targets[i]).AddToQueue();
            }
            // sync attack view to local
            attackView = newAttackView;
        }


        //if (defendView != newDefendView && newAttackView.player1.Equals(PLAYER2))
        //{
            // defendView = newDefendView;
        //}
    }

    // TODO: Sync monster states
    public void SyncMonsters(PlayerArea area, ref List<Monster> playerMonsters, List<Monster> newPlayerMonsters)
    {
        for (int i = 0; i < newPlayerMonsters.Count; i++)
        {
            int currentLife = playerMonsters[i].currentLife;
            int newLife = newPlayerMonsters[i].currentLife;
            if (currentLife != newLife)
            {
                MonsterManager monsterManager = area.monsterVisual.slots.Children[i].GetComponentInChildren<MonsterManager>();
                new ChangeTextCommand(ref monsterManager.LifeText, newLife.ToString()).AddToQueue();
                currentLife = newLife;
            }
        }
    }

    // Show turn messages and enable button
    public void SyncPlayerAndPhase(PlayerArea area, ref string currentPlayer, string newCurrentPlayer, ref string phase, string newPhase)
    {
        if (currentPlayer != newCurrentPlayer || phase != newPhase)
        {
            new ShowMessageCommand(newCurrentPlayer, newPhase, MESSAGE_TIME).AddToQueue();

            // add a commands to highlight playable cards and enable the button
            if (PLAYER1.Equals(newCurrentPlayer))
            {
                StartCoroutine(HighlightPlayableCards());
                new EnableButtonCommand(area).AddToQueue();
            }

            currentPlayer = newCurrentPlayer;
            phase = newPhase;
        }
    }

    // Called on button click
    public void PhaseChange()
    {
        // turn off highlights and disable button
        playerArea.handVisual.turnOffAllHighlights(playerArea);
        playerArea.button.GetComponent<Button>().interactable = false;

        if (GUTS_PHASE.Equals(gameView.phase)) MakeGuts();
        if (DEFEND_PHASE.Equals(gameView.phase)) EndDefend();
        if (ATTACK_PHASE.Equals(gameView.phase)) EndAttack();

        // remove played card visuals from table
        for (int i = 0; i < playerArea.tableVisual.slots.Children.Length; i++)
        {
            new RemoveACardCommand(playerArea, i, false).AddToQueue();
        }
        // remove played card visuals from opponent table
        for (int i = 0; i < opponentArea.tableVisual.slots.Children.Length; i++)
        {
            new RemoveACardCommand(opponentArea, i, false).AddToQueue();
        }

        // reset played cards
        playableRequest.playedCardIndexes = new List<int>();
    }

    // Convert played cards to guts
    private void MakeGuts()
    {
        // update the server
        gutsRequest.discards = playableRequest.playedCardIndexes;
        RESTTemplate.AsyncPOST(SERVER_NAME + GUTS_REQUEST, JsonUtility.ToJson(gutsRequest));
    }

    // End playing defense cards
    private void EndDefend()
    {
        // update the server
        Debug.Log(JsonUtility.ToJson(defendRequest));
        RESTTemplate.AsyncPOST(SERVER_NAME + DEFEND_REQUEST , JsonUtility.ToJson(defendRequest));

        // play attack animation
        new AttackCommand(opponentArea, playerArea, gameView.attackView.user, gameView.attackView.targets[0]).AddToQueue();
        opponentArea.monsterVisual.turnOffAllHighlights(opponentArea);

        // reset defend request
        defendRequest.cardAndTargets = new List<DefendTarget>();
    }

    // End playing attack cards
    private void EndAttack()
    {
        // update server
        Debug.Log(JsonUtility.ToJson(playersRequest));
        RESTTemplate.AsyncPOST(SERVER_NAME + END_ATTACK_REQUEST, JsonUtility.ToJson(playersRequest));
    }

    // Highlight all playable cards in hand
    public IEnumerator HighlightPlayableCards()
    {
        CoroutineWithData cd = new CoroutineWithData(this, RESTTemplate.SyncPOST(SERVER_NAME + PLAYABLE_REQUEST, JsonUtility.ToJson(playableRequest)));
        yield return cd.coroutine;
        string resultBody = (string)cd.result;
        Debug.Log(resultBody);
        playableCardView = JsonUtility.FromJson<PlayableCardView>(resultBody);

        foreach (PlayableCard playableCard in playableCardView.playableCards)
        {
            new HighlightCommand(playerArea, playableCard.cardIndex, false).AddToQueue();
        }
    }

    // Highlight valid users
    public void HighlightPlayableUsers(int cardPlayedIndex)
    {
        // get the last played card
        List<PlayableCard> playableCards = playableCardView.playableCards;
        foreach (int user in playableCards[playableCards.Count-1].users)
        {
            new HighlightCommand(playerArea, user, true).AddToQueue();
        }
    }

    // Highlight valid targets
    public void HighlightPlayableTargets(int cardPlayedIndex)
    {
        // get the last played card
        List<PlayableCard> playableCards = playableCardView.playableCards;
        foreach (int target in playableCards[playableCards.Count - 1].targets)
        {
            new HighlightCommand(playerArea, target, true).AddToQueue();
        }
    }

    // Find a card asset by resource name
    public static CardAsset FindCardAsset(string cardName)
    {
        string[] tokens = cardName.Split('_');
        return Resources.Load<CardAsset>("Cards/" + tokens[0] + "/" + tokens[1]) as CardAsset;
    }

}
