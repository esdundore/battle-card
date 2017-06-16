using System;

[Serializable]
public class Monster
{
    public string id;
    public string type;
    public int maxLife;
    public string mainLineage;
    public string subLineage;
    public int currentLife;
    public bool canAttack;
}
