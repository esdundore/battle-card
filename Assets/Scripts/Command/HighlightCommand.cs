public class HighlightCommand : Command
{
    private PlayerArea playerArea;
    private int index;
    private bool isMonster;

    public HighlightCommand(PlayerArea playerArea, int index, bool isMonster)
    {
        this.playerArea = playerArea;
        this.index = index;
        this.isMonster = isMonster;
    }

    public override void StartCommandExecution()
    {
        if (isMonster)
            playerArea.monsterVisual.HighlightMonster(playerArea, index, true);
        else
            playerArea.handVisual.HighlightCard(playerArea, index, true);
    }
}
