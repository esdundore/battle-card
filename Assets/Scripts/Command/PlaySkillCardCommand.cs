public class PlaySkillCardCommand : Command {

    private PlayerArea playerArea;
    private CardAsset cardAsset;
    private int handIndex;
    private int monsterIndex;

    public PlaySkillCardCommand(PlayerArea playerArea, CardAsset cardAsset, int handIndex, int monsterIndex)
    {
        this.playerArea = playerArea;
        this.cardAsset = cardAsset;
        this.handIndex = handIndex;
        this.monsterIndex = monsterIndex;
    }

    public override void StartCommandExecution()
    {
        playerArea.monsterVisual.AddCard(cardAsset, playerArea.handVisual.FindSlot(handIndex).position, monsterIndex);
    }
}
