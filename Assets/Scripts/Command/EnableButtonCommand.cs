using UnityEngine.UI;

public class EnableButtonCommand : Command
{
    private PlayerArea playerArea;

    public EnableButtonCommand(PlayerArea playerArea)
    {
        this.playerArea = playerArea;
    }

    public override void StartCommandExecution()
    {
        playerArea.button.GetComponent<Button>().interactable = true;
        Command.CommandExecutionComplete();
    }
}
