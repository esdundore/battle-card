using System;

[Serializable]
public class GameView
{
    public int attackId = 0;
    public int defendId = 0;
    public long currentTime;
    public string phase;
    public string currentPlayer;
    public AttackView attackView;
    public DefendView defendView;
    public object environmentCard;
    public PlayerView player;
    public OpponentView opponent;
}
