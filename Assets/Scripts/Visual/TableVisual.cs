using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class TableVisual : MonoBehaviour 
{
    // an enum that mark to whish caracter this table belongs. The alues are - Top or Low
    public AreaPosition owner;

    // a referense to a game object that marks positions where we should put new monsters
    public SameDistanceChildren slots;

    // list of all the cards on the table as GameObjects
    private List<GameObject> CardsOnTable = new List<GameObject>();

    // are we hovering over this table`s collider with a mouse
    private static bool cursorOverThisTable = false;

    // A 3D collider attached to this game object
    private BoxCollider col;

    // returns true only if we are hovering over this table`s collider
    public static bool CursorOverThisTable
    {
        get{ return cursorOverThisTable; }
    }

    // MONOBEHAVIOUR SCRIPTS (mouse over collider detection)
    void Awake()
    {
        col = GetComponent<BoxCollider>();
    }

    // CURSOR/MOUSE DETECTION
    void Update()
    {
        // we need to Raycast because OnMouseEnter, etc reacts to colliders on cards and cards "cover" the table
        // create an array of RaycastHits
        RaycastHit[] hits;
        // raycst to mousePosition and store all the hits in the array
        hits = Physics.RaycastAll(Camera.main.ScreenPointToRay(Input.mousePosition), 30f);

        bool passedThroughTableCollider = false;
        foreach (RaycastHit h in hits)
        {
            // check if the collider that we hit is the collider on this GameObject
            if (h.collider == col)
                passedThroughTableCollider = true;
        }
        cursorOverThisTable = passedThroughTableCollider;
    }

    // creates a card and returns a new card as a GameObject
    GameObject CreateACardAtPosition(CardAsset c, Vector3 position, Vector3 eulerAngles)
    {
        // Instantiate a card depending on its type
        GameObject card;
        // this is a spell: checking for targeted or non-targeted spell
        card = GameObject.Instantiate(GameStateSync.Instance.SkillCardPrefab, position, Quaternion.Euler(eulerAngles)) as GameObject;

        // apply the look of the card based on the info from CardAsset
        SkillCardManager manager = card.GetComponent<SkillCardManager>();
        manager.cardAsset = c;
        manager.ReadCardFromAsset();

        return card;
    }

    // method to add a card to the table
    public void AddCard(CardAsset cardAsset, Vector3 handPosition, int index)
    {
        // create a new card from prefab
        GameObject card = CreateACardAtPosition(cardAsset, handPosition, new Vector3(0f, -179f, 0f)) as GameObject;

        // apply the look from CardAsset
        SkillCardManager manager = card.GetComponent<SkillCardManager>();
        manager.cardAsset = cardAsset;
        manager.ReadCardFromAsset();

        // parent this card to our Slots GameObject
        card.transform.SetParent(slots.Children[index]);

        // Bring card to front while it travels from draw spot to hand
        WhereIsTheCardOrMonster w = card.GetComponent<WhereIsTheCardOrMonster>();
        w.BringToFront();
        w.Slot = index;

        // move card to the hand;
        Sequence s = DOTween.Sequence();
        s.Insert(0f, card.transform.DORotate(Vector3.zero, GameStateSync.Instance.CardTransitionTime));
        s.Append(card.transform.DOMove(slots.Children[index].transform.position, GameStateSync.Instance.CardTransitionTime));

        s.OnComplete(() => Command.CommandExecutionComplete());
    }


    // returns an index for a new skill card based on mousePosition
    // included for placing a new skill card to any positon on the table
    public int TablePosForNewSkill(float MouseX)
    {
        // if there are no monsters or if we are pointing to the right of all monsters with a mouse.
        // right - because the table slots are flipped and 0 is on the right side.
        if (CardsOnTable.Count == 0 || MouseX > slots.Children[0].transform.position.x)
            return 0;
        else if (MouseX < slots.Children[CardsOnTable.Count - 1].transform.position.x) // cursor on the left relative to all monsters on the table
            return CardsOnTable.Count;
        for (int i = 0; i < CardsOnTable.Count; i++)
        {
            if (MouseX < slots.Children[i].transform.position.x && MouseX > slots.Children[i + 1].transform.position.x)
                return i + 1;
        }
        Debug.Log("Suspicious behavior. Reached end of TablePosForNewmonster method. Returning 0");
        return 0;
    }

    // remove card with a given index from table
    public void RemoveCardAtIndex(int index)
    {
        foreach (Transform child in slots.Children[index].transform)
        {
            Destroy(child.gameObject);
        }
        Command.CommandExecutionComplete();
    }

    /// <summary>
    /// Shifts the slots game object according to number of cards.
    /// </summary>
    void ShiftSlotsGameObjectAccordingToNumberOfCards()
    {
        float posX;
        if (CardsOnTable.Count > 0)
            posX = (slots.Children[0].transform.localPosition.x - slots.Children[CardsOnTable.Count - 1].transform.localPosition.x) / 2f;
        else
            posX = 0f;

        slots.gameObject.transform.DOLocalMoveX(posX, 0.3f);  
    }

    /// <summary>
    /// After a card is added or removed, this method
    /// shifts all the cards and places the card on new slots.
    /// </summary>
    void PlaceCardsOnNewSlots()
    {
        foreach (GameObject g in CardsOnTable)
        {
            g.transform.DOLocalMoveX(slots.Children[CardsOnTable.IndexOf(g)].transform.localPosition.x, 0.3f);
            // apply correct sorting order and HandSlot value for later 
            // TODO: figure out if I need to do something here:
            //g.GetComponent<WhereIsTheCardOrMonster>().SetTableSortingOrder() = CardsOnTable.IndexOf(g);
        }
    }

}
