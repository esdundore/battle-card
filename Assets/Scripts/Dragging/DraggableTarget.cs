using UnityEngine;
using System.Collections.Generic;

public class DraggableTarget : DraggingActions
{
    private SpriteRenderer sr;
    private LineRenderer lr;
    private Transform triangle;
    private SpriteRenderer triangleSR;
    private GameObject Target;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        lr = GetComponentInChildren<LineRenderer>();
        lr.sortingLayerName = "Above Everything";
        triangle = transform.Find("Triangle");
        triangleSR = triangle.GetComponent<SpriteRenderer>();
    }

    public override void OnStartDrag()
    {
        sr.enabled = true;
        lr.enabled = true;
        MonsterManager monsterManager = this.transform.parent.GetComponent<MonsterManager>();

        //add this user and card played to defend request
        if (GameStateSync.Instance.gameView.phase.Equals(GameStateSync.DEFEND_PHASE))
        {
            DefendTarget defendTarget = new DefendTarget();
            List<int> cardsPlayed = GameStateSync.Instance.playableRequest.playedCardIndexes;
            defendTarget.card = cardsPlayed[cardsPlayed.Count - 1]; // last card played
            defendTarget.user = monsterManager.index;
            GameStateSync.Instance.defendRequest.cardAndTargets.Add(defendTarget);
            GameStateSync.Instance.playerArea.monsterVisual.turnOffAllHighlights(GameStateSync.Instance.playerArea);
        }
    }

    public override void OnDraggingInUpdate()
    {
        // This code only draws the arrow
        Vector3 notNormalized = transform.position - transform.parent.position;
        Vector3 direction = notNormalized.normalized;
        float distanceToTarget = (direction*2.3f).magnitude;
        if (notNormalized.magnitude > distanceToTarget)
        {
            // draw a line between the creature and the target
            lr.SetPositions(new Vector3[]{ transform.parent.position, transform.position - direction*2.3f });
            lr.enabled = true;

            // position the end of the arrow between near the target.
            triangleSR.enabled = true;
            triangleSR.transform.position = transform.position - 1.5f*direction;

            // proper rotarion of arrow end
            float rot_z = Mathf.Atan2(notNormalized.y, notNormalized.x) * Mathf.Rad2Deg;
            triangleSR.transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);
        }
        else
        {
            // if the target is not far enough from creature, do not show the arrow
            lr.enabled = false;
            triangleSR.enabled = false;
        }

    }

    public override void OnEndDrag()
    {

        transform.localPosition = new Vector3(0f, 0f, 0.4f);
        sr.enabled = false;
        lr.enabled = false;
        triangleSR.enabled = false;

    }

    protected override bool DragSuccessful()
    {
        return true;
    }
}
