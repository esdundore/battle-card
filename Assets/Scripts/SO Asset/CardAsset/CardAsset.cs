using UnityEngine;

public class CardAsset : ScriptableObject 
{
    public string id;
    public MonsterAsset monsterAsset;
    public Sprite image;
    public int gutsCost;
    public string user;
    public string type;
    public int damage;
    [TextArea(2, 3)]
    public string description;
}
