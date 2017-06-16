using UnityEngine;

public class Draggable : MonoBehaviour {

    // a flag to know if we are currently dragging this GameObject
    private bool dragging = false;

    // distance from the center of this Game Object to the point where we clicked to start dragging 
    private Vector3 pointerDisplacement;
    private float zDisplacement;

    // reference to DraggingActions script - Dragging Actions should be attached to the same GameObject
    private DraggingActions da;
    private static Draggable draggingThis;
    public static Draggable DraggingThis
    {
        get{ return draggingThis;}
    }

    void Awake()
    {
        da = GetComponent<DraggingActions>();
    }

    void OnMouseDown()
    {
        bool isHighlighted = false;
        MonsterManager monsterManager = this.transform.parent.GetComponent<MonsterManager>();
        SkillCardManager skillCardManager = this.transform.GetComponent<SkillCardManager>();
        isHighlighted = monsterManager != null ? monsterManager.CardFaceGlowImage.enabled : skillCardManager.CardFaceGlowImage.enabled;

        if (da != null && da.CanDrag() && isHighlighted)
        {
            dragging = true;
            draggingThis = this;
            HoverPreview.PreviewsAllowed = false;
            zDisplacement = -Camera.main.transform.position.z + transform.position.z;
            pointerDisplacement = -transform.position + MouseInWorldCoords();

            da.OnStartDrag();
        }

    }

    void Update ()
    {
        if (dragging)
        { 
            Vector3 mousePos = MouseInWorldCoords();
            transform.position = new Vector3(mousePos.x - pointerDisplacement.x, mousePos.y - pointerDisplacement.y, transform.position.z);   

            da.OnDraggingInUpdate();
        }
    }
	
    void OnMouseUp()
    {
        if (dragging)
        {
            dragging = false;
            draggingThis = null;
            HoverPreview.PreviewsAllowed = true;

            da.OnEndDrag();
        }
    }   

    // returns mouse position in World coordinates for our GameObject to follow
    private Vector3 MouseInWorldCoords()
    {
        var screenMousePos = Input.mousePosition;
        screenMousePos.z = zDisplacement;
        return Camera.main.ScreenToWorldPoint(screenMousePos);
    }
        
}
