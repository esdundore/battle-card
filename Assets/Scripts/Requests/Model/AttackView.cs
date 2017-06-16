using System;
using System.Collections.Generic;

[Serializable]
public class AttackView
{
    public string player1;
    public string player2;
    public int user;
    public List<string> cardsPlayed;
    public List<int> handIndexes;
    public List<int> targets;
    public List<int> damage;
}
