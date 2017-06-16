using System;
using System.Collections.Generic;

[Serializable]
public class PlayableCard
{
    public int cardIndex;
    public List<int> users;
    public List<int> targets;
}
