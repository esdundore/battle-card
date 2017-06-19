public class RemoveACardCommand : Command {

    private PlayerArea playerArea;
    private bool fromHand;
    private int handPos;
    private int monsterPos;
    private int skillPos;

    public RemoveACardCommand(PlayerArea playerArea, int handPos)
    {
        this.playerArea = playerArea;
        this.handPos = handPos;
        this.fromHand = true;
    }

    public RemoveACardCommand(PlayerArea playerArea, int monsterPos, int skillPos)
    {
        this.playerArea = playerArea;
        this.monsterPos = monsterPos;
        this.skillPos = skillPos;
        this.fromHand = false;
    }

    public override void StartCommandExecution()
    {
        //remove from hand
        if (fromHand)
            playerArea.handVisual.RemoveCardAtIndex(handPos);
        //remove from table
        else
            playerArea.monsterVisual.RemoveCardAtIndex(monsterPos, skillPos);

    }
}
