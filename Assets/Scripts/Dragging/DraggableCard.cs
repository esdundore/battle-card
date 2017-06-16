using UnityEngine;
using DG.Tweening;

public class DraggableCard : DraggingActions {

    private int savedHandSlot;
    private WhereIsTheCardOrMonster whereIsCard;
    private VisualStates tempState;
    private GameObject Target;

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

        if (GameStateSync.Instance.gameView.phase.Equals(GameStateSync.GUTS_PHASE))
        {
            GameStateSync.Instance.playerArea.monsterVisual.avatarManager.CardFaceGlowImage.enabled = true;
        }
    }

    public override void OnDraggingInUpdate()
    {

    }

    public override void OnEndDrag()
    {
        // turn off user monster highlights
        MonsterVisual monsterVisual = GameStateSync.Instance.playerArea.monsterVisual;
        monsterVisual.turnOffAllHighlights(GameStateSync.Instance.playerArea);

        GameObject target = DragTarget();

        if (target != null)
        {
            // turn off hand highlights
            HandVisual handVisual = GameStateSync.Instance.playerArea.handVisual;
            handVisual.turnOffAllHighlights(GameStateSync.Instance.playerArea);

            // play this card
            SameDistanceChildren skillSlots = gameObject.transform.parent.GetComponent<SameDistanceChildren>();
            Transform openSkillSlot = null;
            // find first open slot
            foreach (Transform skillSlot in skillSlots.Children)
            {
                if(skillSlot.childCount == 0)
                {
                    openSkillSlot = skillSlot;
                    break;
                }
            }
            transform.SetParent(openSkillSlot);
            whereIsCard.SetHandSortingOrder();
            whereIsCard.VisualState = tempState;
            transform.DOMove(openSkillSlot.transform.position, 1f);

            GameStateSync.Instance.playableRequest.playedCardIndexes.Add(savedHandSlot);

            if (GameStateSync.Instance.gameView.phase == GameStateSync.GUTS_PHASE)
            {
                // discard this
                // increment guts
                StartCoroutine(GameStateSync.Instance.HighlightPlayableCards());
            }
            if (GameStateSync.Instance.gameView.phase == GameStateSync.ATTACK_PHASE)
            {
                GameStateSync.Instance.HighlightPlayableUsers(savedHandSlot);
            }
            if (GameStateSync.Instance.gameView.phase == GameStateSync.DEFEND_PHASE)
            {
                GameStateSync.Instance.HighlightPlayableUsers(savedHandSlot);
            }
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
            if (monsterManager != null && monsterManager.CardFaceGlowImage.enabled)
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
