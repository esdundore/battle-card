using UnityEngine;

public class PhaseButton : MonoBehaviour {

    public AreaPosition owner;

    public GameObject Front;
    public GameObject Back;

    public GameObject Glow;

    private bool wasUsed = false;
    public bool WasUsedThisTurn
    { 
        get
        {
            return wasUsed;
        } 
        set
        {
            wasUsed = value;
            if (!wasUsed)
            {
                Front.SetActive(true);
                Back.SetActive(false);
            }
            else
            {
                Front.SetActive(false);
                Back.SetActive(true);
                Highlighted = false;
            }
        }
    }

    private bool highlighted = false;
    public bool Highlighted
    {
        get{ return highlighted; }

        set
        {
            highlighted = value;
            Glow.SetActive(highlighted);
        }
    }

    void OnMouseDown()
    {
        //if (Highlighted)
        //{
        Debug.Log("clicked");
        GameStateSync.Instance.PhaseChange();
        //}
    }
}
