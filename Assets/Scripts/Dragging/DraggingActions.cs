using UnityEngine;

public abstract class DraggingActions : MonoBehaviour {

    public abstract void OnStartDrag();

    public abstract void OnEndDrag();

    public abstract void OnDraggingInUpdate();

    protected abstract bool DragSuccessful();

    public bool CanDrag()
    {
        return true;
    }

}
