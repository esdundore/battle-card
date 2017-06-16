using UnityEngine;
using UnityEngine.UI;

// this class should be attached to the deck
// generates new cards and places them into the hand
public class PlayerDeckVisual : MonoBehaviour {

    public AreaPosition owner;
    public float HeightOfOneCard = 0.012f;
    public Text DeckSizeText;

    void Start()
    {

    }

    private int cardsInDeck = 0;
    public int CardsInDeck
    {
        get{ return cardsInDeck; }

        set
        {
            cardsInDeck = value;
            transform.position = new Vector3(transform.position.x, transform.position.y, - HeightOfOneCard * value);
        }
    }
   
}
