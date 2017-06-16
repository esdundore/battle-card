using UnityEngine.UI;

public class ChangeTextCommand : Command
{
    private Text oldText;
    private string newText;

    public ChangeTextCommand(ref Text oldText, string newText)
    {
        this.oldText = oldText;
        this.newText = newText;
    }

    public override void StartCommandExecution()
    {
        oldText.text = newText;
        Command.CommandExecutionComplete();
    }
}
