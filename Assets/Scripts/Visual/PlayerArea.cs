using UnityEngine;

public enum AreaPosition{Top, Low}

public class PlayerArea : MonoBehaviour 
{
    public AreaPosition owner;
    public HandVisual handVisual;
    public MonsterVisual monsterVisual;
    public TableVisual tableVisual;
    public PlayerDeckVisual deckVisual;
    public Transform button;
    public Color32 highlightColor;
}
