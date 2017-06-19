using UnityEngine;
using DG.Tweening;
using System;

public class DraggableCard : DraggingActions {

    private int savedHandSlot;
    private WhereIsTheCardOrMonster whereIsCard;
    private VisualStates tempState;

    void Awake()
    {
        whereIsCard = GetComponent<WhereIsTheCardOrMonster>();
    }

    public override void OnStartDrag()
    {
        savedHandSlot = whereIsCard.Slot;
        tempState = whereIsCard.VisualState;
        whereIsCard.VisualState = VisualStates.Dragging;
        whereIsCard.BringToFront();

        PlayerArea area = GameStateSync.Instance.playerArea;
        if (GameStateSync.Instance.gameView.phase.Equals(GameStateSync.GUTS_PHASE))
        {
            area.monsterVisual.HighlightMonster(area.highlightColor, 3, true);
        }
        else if (GameStateSync.Instance.gameView.phase.Equals(GameStateSync.ATTACK_PHASE)
            || GameStateSync.Instance.gameView.phase.Equals(GameStateSync.DEFEND_PHASE))
        {
            foreach (PlayableCard playableCard in GameStateSync.Instance.playableCardView.playableCards)
            {
                if (playableCard.cardIndex == savedHandSlot)
                {
                    for (int i = 0; i < playableCard.users.Count; i++)
                    {
                        area.monsterVisual.HighlightMonster(area.highlightColor, playableCard.users[i], true);
                        GameStateSync.Instance.targets = playableCard.targets;
                    }
                }
            }
        }
    }

    public override void OnDraggingInUpdate()
    {

    }

    public override void OnEndDrag()
    {
        // determine if over a valid target
        GameObject target = DragTarget();

        // turn off user monster highlights
        PlayerArea area = GameStateSync.Instance.playerArea;
        MonsterVisual monsterVisual = area.monsterVisual;
        monsterVisual.turnOffAllHighlights();

        if (target != null)
        {
            // turn off hand highlights
            HandVisual handVisual = GameStateSync.Instance.playerArea.handVisual;
            handVisual.turnOffAllHighlights(GameStateSync.Instance.playerArea);

            // target monster index 
            MonsterManager monsterManager = target.GetComponent<MonsterManager>();
            AvatarManager avatarManager = target.GetComponentInChildren<AvatarManager>();
            int targetIndex = monsterManager == null ? 3 : monsterManager.index;

            // find first open skill slot
            Transform openSkillSlot = monsterVisual.FindOpenSlot(targetIndex);

            transform.SetParent(openSkillSlot);
            whereIsCard.SetHandSortingOrder();
            whereIsCard.VisualState = tempState;
            transform.DOMove(openSkillSlot.position, 1f);

            GameStateSync.Instance.playableRequest.playedCardIndexes.Add(savedHandSlot);

            if (GameStateSync.Instance.gameView.phase == GameStateSync.GUTS_PHASE)
            {
                // increment guts
                avatarManager.gutsText.text = (Int32.Parse(avatarManager.gutsText.text) + 1).ToString(); 
                // make this invisible
                // this.transform.parent.gameObject.GetComponentInChildren<CanvasRenderer>().SetAlpha(0);
            }
            if (GameStateSync.Instance.gameView.phase == GameStateSync.ATTACK_PHASE)
            {
                // add to cards played
                GameStateSync.Instance.attackRequest.user = monsterManager.index;
                GameStateSync.Instance.attackRequest.cardsPlayed.Add(savedHandSlot);
                // highlight user to attack
                area.monsterVisual.HighlightMonster(area.highlightColor, monsterManager.index, true);
            }
            if (GameStateSync.Instance.gameView.phase == GameStateSync.DEFEND_PHASE)
            {
                // add to defend request
                DefendTarget defendTarget = new DefendTarget();
                defendTarget.card = savedHandSlot;
                defendTarget.user = monsterManager.index;
                GameStateSync.Instance.defendRequest.cardAndTargets.Add(defendTarget);
            }

            // highlight other playable cards
            StartCoroutine(GameStateSync.Instance.HighlightPlayableCards());
        }
        else
        {
            // Set old sorting order 
            whereIsCard.SetHandSortingOrder();
            whereIsCard.VisualState = tempState;
            // Move this card back to its slot position
            HandVisual PlayerHand = GameStateSync.Instance.playerArea.handVisual;
            Vector3 oldCardPos = PlayerHand.slots.Children[savedHandSlot].transform.position;
            transform.DOMove(oldCardPos, 1f);
        } 
    }

    // check if hovering over a highlighted monster or avatar
    private GameObject DragTarget()
    {

        GameObject target = null;
        RaycastHit[] hits;
        hits = Physics.RaycastAll(origin: Camera.main.transform.position,
            direction: (-Camera.main.transform.position + this.transform.position).normalized,
            maxDistance: 30f);

        foreach (RaycastHit hit in hits)
        {
            GameObject gameObject = hit.transform.parent.gameObject;
            MonsterManager monsterManager = gameObject.GetComponent<MonsterManager>();
            AvatarManager avatarManager = gameObject.GetComponentInChildren<AvatarManager>();
            if ((monsterManager != null && monsterManager.CardFaceGlowImage.enabled) ||
                (avatarManager != null && avatarManager.CardFaceGlowImage.enabled))
            {
                target = gameObject;
                break;
            }
        }

        return target;
    }

    protected override bool DragSuccessful()
    {
        return true;
    }

}
