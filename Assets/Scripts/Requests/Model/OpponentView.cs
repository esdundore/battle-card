using System;
using System.Collections.Generic;

[Serializable]
public class OpponentView
{
    public int gutsPool;
    public List<string> hand;
    public List<Monster> monsters;
    public int deckSize;
}
