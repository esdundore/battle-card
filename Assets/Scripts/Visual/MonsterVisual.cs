using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class MonsterVisual : MonoBehaviour
{
    // PUBLIC FIELDS
    public AreaPosition owner;
    public SameDistanceChildren slots;
    public List<SameDistanceChildren> skillSlots;

    private static List<LineRenderer> arrows = new List<LineRenderer>();

    public List<GameObject> monsterObjects = new List<GameObject>();
    public List<MonsterManager> monsterManagers;

    public GameObject avatarObject;
    public AvatarManager avatarManager;

    public void Start()
    {
        // create the avatar at the last open monster slot
        avatarObject = GameObject.Instantiate(GameStateSync.Instance.AvatarPrefab, 
            slots.Children[slots.Children.Length-1].transform.position, 
            Quaternion.Euler(new Vector3(0f, 0f, 0f))) as GameObject;
        avatarObject.transform.SetParent(slots.Children[slots.Children.Length - 1]);
        avatarManager = avatarObject.GetComponent<AvatarManager>();
        //manager.ReadCardFromAsset();
        avatarManager.CardFaceGlowImage.enabled = false;
    }

    // add a new monster GameObject
    public void AddMonster(GameObject monster, int index)
    {
        // we allways insert a new card as 0th element in CardsInHand List 
        monsterObjects.Insert(index, monster);

        // parent this card to our Slots GameObject
        monster.transform.SetParent(slots.Children[index]);
    }

    // creates a monster and returns a new monster as a GameObject
    GameObject CreateAMonsterAtPosition(MonsterAsset monsterAsset, Vector3 position, Vector3 eulerAngles)
    {
        // Instantiate a card depending on its type
        GameObject monster;

        // this is a spell: checking for targeted or non-targeted spell
        monster = GameObject.Instantiate(GameStateSync.Instance.MonsterCardPrefab, position, Quaternion.Euler(eulerAngles)) as GameObject;

        // apply the look of the monster
        MonsterManager manager = monster.GetComponent<MonsterManager>();
        manager.monsterAsset = monsterAsset;
        manager.ReadCardFromAsset();

        return monster;
    }

    // flips a player's monster card
    public void PlayMonster(MonsterAsset monsterAsset, int index)
    {
        GameObject monster;
        monster = CreateAMonsterAtPosition(monsterAsset, slots.Children[index].position, new Vector3(0f, -179f, 0f));
        MonsterManager monsterManager = monster.GetComponent<MonsterManager>();
        monsterManager.index = index;
        monsterManager.CardFaceGlowImage.enabled = false;
        monsterManagers.Insert(index, monsterManager);

        // pass this monster to HandVisual class
        AddMonster(monster, index);

        // bring monster to front
        WhereIsTheCardOrMonster w = monster.GetComponent<WhereIsTheCardOrMonster>();
        w.BringToFront();
        w.Slot = index; 

        // flip monster
        Sequence s = DOTween.Sequence();
        s.Insert(0f, monster.transform.DORotate(Vector3.zero, GameStateSync.Instance.MonsterFlipTime));

        // displace the monster so that we can select it in the scene easier.
        s.Append(monster.transform.DOLocalMove(slots.Children[0].transform.localPosition, GameStateSync.Instance.MonsterFlipTime));

        s.OnComplete(() => ChangeLastCardStatusToInHand(monster, w));
    }

    // this method will be called when the monster is played
    void ChangeLastCardStatusToInHand(GameObject monster, WhereIsTheCardOrMonster w)
    {
        if (owner == AreaPosition.Low)
            w.VisualState = VisualStates.LowHand;
        else
            w.VisualState = VisualStates.TopHand;

        // set correct sorting order
        w.SetHandSortingOrder();
        // end command execution
        Command.CommandExecutionComplete();
    }

    // highlight a monster card
    public void HighlightMonster(Color32 highlightColor, int monsterIndex, bool isEnabled)
    {
        Transform child = slots.Children[monsterIndex];
        MonsterManager monsterManager = child.GetComponentInChildren<MonsterManager>();
        if (monsterManager == null)
        {
            avatarManager.CardFaceGlowImage.color = highlightColor;
            avatarManager.CardFaceGlowImage.enabled = isEnabled;
        }
        else
        {
            monsterManager.CardFaceGlowImage.color = highlightColor;
            monsterManager.CardFaceGlowImage.enabled = isEnabled;
        }
        Command.CommandExecutionComplete();
    }
    // turn off all highlights
    public void turnOffAllHighlights()
    {
        for (int i = 0; i < slots.Children.Length; i++)
        {
            if (slots.Children[i].transform != null)
            {
                HighlightMonster(new Color32(), i, false);
            }
        }
    }

    public void DrawArrow(PlayerArea targetArea, int user, int target)
    {
        GameObject targetArrow = GameObject.Instantiate(GameStateSync.Instance.ArrowTarget) as GameObject;
        Vector3 startVector = slots.Children[user].transform.position;
        Vector3 endVector = targetArea.monsterVisual.slots.Children[target].transform.position;
        LineRenderer lr = targetArrow.GetComponentInChildren<LineRenderer>();
        if (lr == null)
        {
            Debug.Log("something wrong");
        }
        lr.SetPositions(new Vector3[] { startVector, endVector });
        lr.sortingLayerName = "Above Everything";
        lr.enabled = true;
        arrows.Add(lr);
    }

    public void AttackSequence(int attackerIndex, int defenderIndex, MonsterVisual otherMonsterVisual)
    {
        // Delete arrows
        foreach (LineRenderer lr in arrows)
        {
            Destroy(lr);
        }

        MonsterManager attacker = slots.Children[attackerIndex].GetComponentInChildren<MonsterManager>();
        Vector3 defenderPosition = otherMonsterVisual.slots.Children[defenderIndex].transform.position;
        Vector3 originalPosition = slots.Children[attackerIndex].transform.position;

        Sequence s = DOTween.Sequence();
        s.Append(attacker.transform.DOMove(defenderPosition, 0.5f).SetEase(Ease.InQuint));
        s.Append(attacker.transform.DOMove(originalPosition, 0.5f));

    }

    // remove card with a given index from table
    public void RemoveCardAtIndex(int monsterPos, int skillPos)
    {
        foreach (Transform monsterChild in slots.Children[monsterPos].transform)
        {
            foreach (Transform child in monsterChild.GetComponentInParent<SameDistanceChildren>().Children[skillPos])
            {
                Destroy(child.gameObject);
            }
        }
        Command.CommandExecutionComplete();
    }

    // method to add a card to the table
    public void AddCard(CardAsset cardAsset, Vector3 handPosition, int monsterIndex)
    {
        // create a new card from prefab
        GameObject card;
        card = GameObject.Instantiate(GameStateSync.Instance.SkillCardPrefab, handPosition, Quaternion.Euler(new Vector3(0f, -179f, 0f))) as GameObject;

        // apply the look of the card based on the info from CardAsset
        SkillCardManager manager = card.GetComponent<SkillCardManager>();
        manager.cardAsset = cardAsset;
        manager.ReadCardFromAsset();

        // parent this card to our open slot
        Transform openSlot = FindOpenSlot(monsterIndex);
        card.transform.SetParent(openSlot);

        // Bring card to front while it travels from draw spot to hand
        WhereIsTheCardOrMonster w = card.GetComponent<WhereIsTheCardOrMonster>();
        w.BringToFront();
        w.Slot = monsterIndex;

        // move card to the hand;
        Sequence s = DOTween.Sequence();
        s.Insert(0f, card.transform.DORotate(Vector3.zero, GameStateSync.Instance.CardTransitionTime));
        s.Append(card.transform.DOMove(openSlot.position, GameStateSync.Instance.CardTransitionTime));

        s.OnComplete(() => Command.CommandExecutionComplete());
    }

    public Transform FindOpenSlot(int monsterIndex)
    {
        Transform monsterSlot = slots.Children[monsterIndex];
        foreach (Transform skillSlot in monsterSlot.GetComponentInParent<SameDistanceChildren>().Children)
            if (skillSlot.childCount == 0) return skillSlot;
        return null;
    }

}
