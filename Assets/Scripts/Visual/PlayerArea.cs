using UnityEngine;

public enum AreaPosition{Top, Low}

public class PlayerArea : MonoBehaviour 
{
    public AreaPosition owner;
    public PlayerDeckVisual deckVisual;
    public HandVisual handVisual;
    public Transform button;
    public TableVisual tableVisual;
    public AvatarManager avatar;
    public MonsterVisual monsterVisual;  

}
