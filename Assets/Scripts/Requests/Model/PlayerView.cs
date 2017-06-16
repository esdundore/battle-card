using System;
using System.Collections.Generic;

[Serializable]
public class PlayerView
{
    public int gutsPool;
    public List<string> hand;
    public List<string> discard;
    public List<Monster> monsters;
    public int deckSize;
}
