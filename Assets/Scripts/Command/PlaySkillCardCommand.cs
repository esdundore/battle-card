public class PlaySkillCardCommand : Command {

    private PlayerArea playerArea;
    private CardAsset cardAsset;
    private int index;

    public PlaySkillCardCommand(PlayerArea playerArea, CardAsset cardAsset, int index)
    {
        this.playerArea = playerArea;
        this.cardAsset = cardAsset;
        this.index = index;
    }

    public override void StartCommandExecution()
    {
        playerArea.tableVisual.AddCard(cardAsset, playerArea.handVisual.slots.Children[index].transform.position, index);
    }
}
