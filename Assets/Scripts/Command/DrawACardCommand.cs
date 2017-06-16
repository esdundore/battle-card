public class DrawACardCommand : Command {

    private PlayerArea playerArea;
    private CardAsset cardAsset;
    private int handPos;
    private bool canSee;

    public DrawACardCommand(PlayerArea playerArea, CardAsset cardAsset, int handPos, bool canSee)
    {
        this.playerArea = playerArea;
        this.cardAsset = cardAsset;
        this.handPos = handPos;
        this.canSee = canSee;
    }

    public override void StartCommandExecution()
    {
        playerArea.handVisual.GivePlayerACard(cardAsset, handPos, canSee);
    }
}
