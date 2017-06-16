public class DrawArrowCommand : Command
{
    private PlayerArea playerArea;
    private PlayerArea targetArea;
    private int user;
    private int target;

    public DrawArrowCommand(PlayerArea playerArea, PlayerArea targetArea, int user, int target)
    {
        this.playerArea = playerArea;
        this.targetArea = targetArea;
        this.user = user;
        this.target = target;
    }

    public override void StartCommandExecution()
    {
        playerArea.monsterVisual.DrawArrow(playerArea, targetArea, user, target);
        Command.CommandExecutionComplete();
    }
}
