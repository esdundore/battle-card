public class AttackCommand : Command
{
    private PlayerArea playerArea;
    private PlayerArea otherPlayerArea;
    private int attackerIndex;
    private int defenderIndex;


    public AttackCommand(PlayerArea playerArea, PlayerArea otherPlayerArea, int attackerIndex, int defenderIndex)
    {
        this.playerArea = playerArea;
        this.otherPlayerArea = otherPlayerArea;
        this.attackerIndex = attackerIndex;
        this.defenderIndex = defenderIndex;
    }

    public override void StartCommandExecution()
    {
        playerArea.monsterVisual.AttackSequence(attackerIndex, defenderIndex, otherPlayerArea.monsterVisual);
        Command.CommandExecutionComplete();
    }
}
