using System;
using System.Collections.Generic;

[Serializable]
public class PlayableRequest : PlayersRequest
{

    public List<int> playedCardIndexes;

}
