public class PlayAMonsterCommand : Command {

    private PlayerArea playerArea;
    private MonsterAsset monsterAsset;
    private int index;

    public PlayAMonsterCommand(PlayerArea playerArea, MonsterAsset monsterAsset, int index)
    {
        this.playerArea = playerArea;
        this.monsterAsset = monsterAsset;
        this.index = index;
    }

    public override void StartCommandExecution()
    {
        playerArea.monsterVisual.PlayMonster(monsterAsset, index);
    }
}
