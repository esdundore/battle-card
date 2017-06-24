using UnityEngine;

public class DraggableTarget : DraggingActions
{
    private SpriteRenderer sr;
    private LineRenderer lr;
    private Transform triangle;
    private SpriteRenderer triangleSR;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        lr = GetComponentInChildren<LineRenderer>();
        lr.sortingLayerName = GameStateSync.ABOVE_EVERYTHING_LAYER;
        triangle = transform.Find("Triangle");
        triangleSR = triangle.GetComponent<SpriteRenderer>();
    }

    public override void OnStartDrag()
    {
        sr.enabled = true;
        lr.enabled = true;

        // turn off user monster highlights
        PlayerArea playerArea = GameStateSync.Instance.playerArea;
        playerArea.monsterVisual.turnOffAllHighlights();

        // highlight valid targets
        PlayerArea area = GameStateSync.Instance.opponentArea;
        foreach (int target in GameStateSync.Instance.targets)
        {
            area.monsterVisual.HighlightMonster(area.highlightColor, target, true);
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
        GameObject target = DragTarget();

        // turn off user monster highlights
        PlayerArea area = GameStateSync.Instance.opponentArea;
        area.monsterVisual.turnOffAllHighlights();

        if (target != null)
        {
            MonsterManager monsterManager = target.GetComponent<MonsterManager>();
            // set attack target
            GameStateSync.Instance.attackRequest.targets.Add(monsterManager.index);
            GameStateSync.Instance.attackRequest.damages.Add(0);
            // send attack request
            GameStateSync.Instance.MakeAttack();
        }
        else
        {
            // highlight monster again
            GetComponentInParent<MonsterManager>().CardFaceGlowImage.enabled = true;
        }

        transform.localPosition = new Vector3(0f, 0f, 0.4f);
        sr.enabled = false;
        lr.enabled = false;
        triangleSR.enabled = false;

    }

    // check if dragged over a highlighted monster
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
            if ((monsterManager != null && monsterManager.CardFaceGlowImage.enabled))
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
