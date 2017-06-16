public class ShowMessageCommand : Command {

    string player;
    string phase;
    float duration;

    public ShowMessageCommand(string player, string phase, float duration)
    {
        this.player = player;
        this.phase = phase;
        this.duration = duration;
    }

    public override void StartCommandExecution()
    {
        MessageManager.Instance.ShowMessage(player, phase, duration);
    }
}
