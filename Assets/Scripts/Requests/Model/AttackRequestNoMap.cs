using System;
using System.Collections.Generic;

[Serializable]
public class AttackRequestNoMap : PlayersRequest
{

    public int user;
    public List<String> cardNames;
    public List<int> cardsPlayed;
    public List<int> targets;
    public List<int> damages;

}
