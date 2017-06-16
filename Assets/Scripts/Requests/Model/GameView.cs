using System;

[Serializable]
public class GameView
{
    public long currentTime;
    public string phase;
    public string currentPlayer;
    public AttackView attackView;
    public DefendView defendView;
    public object environmentCard;
    public PlayerView player;
    public OpponentView opponent;
}
