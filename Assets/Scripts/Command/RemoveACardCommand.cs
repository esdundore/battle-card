public class RemoveACardCommand : Command {

    private PlayerArea playerArea;
    private int handPos;
    private bool fromHand;

    public RemoveACardCommand(PlayerArea playerArea, int handPos, bool fromHand)
    {
        this.playerArea = playerArea;
        this.handPos = handPos;
        this.fromHand = fromHand;
    }

    public override void StartCommandExecution()
    {
        if (fromHand)
        {
            playerArea.handVisual.RemoveCardAtIndex(handPos);
        }
        else
        {
            playerArea.tableVisual.RemoveCardAtIndex(handPos);
        }
    }
}
