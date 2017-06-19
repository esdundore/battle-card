using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class HandVisual : MonoBehaviour
{
    // PUBLIC FIELDS
    public AreaPosition owner;
    public bool TakeCardsOpenly = true;
    public SameDistanceChildren slots;

    [Header("Transform References")]
    public Transform DrawPreviewSpot;
    public Transform DeckTransform;
    public Transform OtherCardDrawSourceTransform;
    public Transform PlayPreviewSpot;

    // PRIVATE : a list of all card visual representations as GameObjects
    private List<GameObject> cardsInHand = new List<GameObject>();

    // ADDING OR REMOVING CARDS FROM HAND

    // add a new card GameObject to hand
    public void AddCard(GameObject card, int index)
    {
        // we allways insert a new card as 0th element in CardsInHand List 
        cardsInHand.Insert(index, card);

        // parent this card to our Slots GameObject
        card.transform.SetParent(slots.Children[index]);
    }

    // remove a card GameObject from hand
    public void RemoveCard(GameObject card)
    {
        // remove a card from the list
        cardsInHand.Remove(card);
    }

    // remove card with a given index from hand
    public void RemoveCardAtIndex(int index)
    {
        //cardsInHand.RemoveAt(index);
        foreach (Transform transform in slots.Children[index].transform)
        {
            Destroy(transform.gameObject);
        }
        Command.CommandExecutionComplete();
    }

    // get a card GameObject with a given index in hand
    public GameObject GetCardAtIndex(int index)
    {
        return cardsInHand[index];
    }

    // CARD DRAW METHODS

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
        manager.CardFaceGlowImage.enabled = false;

        return card;
    }

    // gives player a new card from a given position
    public void GivePlayerACard(CardAsset c, int index, bool canSee)
    {
        GameObject card;
        card = CreateACardAtPosition(c, DeckTransform.position, new Vector3(0f, -179f, 0f));

        // pass this card to HandVisual class
        AddCard(card, index);

        // Bring card to front while it travels from draw spot to hand
        WhereIsTheCardOrMonster w = card.GetComponent<WhereIsTheCardOrMonster>();
        w.BringToFront();
        w.Slot = index; 

        // move card to the hand;
        Sequence s = DOTween.Sequence();

        // Debug.Log ("Not fast!!!");
        if (canSee)
        {
            s.Append(card.transform.DOMove(DrawPreviewSpot.position, GameStateSync.Instance.CardTransitionTime));

            if (TakeCardsOpenly)
                s.Insert(0f, card.transform.DORotate(Vector3.zero, GameStateSync.Instance.CardTransitionTime));
            else
                s.Insert(0f, card.transform.DORotate(new Vector3(0f, 179f, 0f), GameStateSync.Instance.CardTransitionTime));
            s.AppendInterval(GameStateSync.Instance.CardPreviewTime);
        }

        // displace the card so that we can select it in the scene easier.
        s.Append(card.transform.DOLocalMove(slots.Children[0].transform.localPosition, GameStateSync.Instance.CardTransitionTime));

        s.OnComplete(()=>ChangeLastCardStatusToInHand(card, w));
    }

    // this method will be called when the card arrived to hand 
    void ChangeLastCardStatusToInHand(GameObject card, WhereIsTheCardOrMonster w)
    {
        //Debug.Log("Changing state to Hand for card: " + card.gameObject.name);
        if (owner == AreaPosition.Low)
            w.VisualState = VisualStates.LowHand;
        else
            w.VisualState = VisualStates.TopHand;

        // set correct sorting order
        w.SetHandSortingOrder();
        // end command execution for DrawACArdCommand
        Command.CommandExecutionComplete();
    }

    // highlight a card in hand
    public void HighlightCard(Color32 highlightColor, int index, bool isTurnOn)
    {
        foreach (Transform child in slots.Children[index].transform)
        {
            //highlight
            SkillCardManager skillCardManager = child.gameObject.GetComponent<SkillCardManager>();
            skillCardManager.CardFaceGlowImage.color = highlightColor;
            skillCardManager.CardFaceGlowImage.enabled = isTurnOn;
        }
        Command.CommandExecutionComplete();
    }

    // turn off all highlights
    public void turnOffAllHighlights(PlayerArea area)
    {
        for (int i = 0; i < slots.Children.Length; i++)
        {
            if (slots.Children[i].transform != null)
            {
                HighlightCard(new Color32(), i, false);
            }
        }
    }

    public Transform FindSlot(int index)
    {
        return slots.Children[index];
    }

}
