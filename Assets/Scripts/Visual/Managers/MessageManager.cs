using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MessageManager : MonoBehaviour 
{
    public Text MessageText;
    public GameObject MessagePanel;

    public static MessageManager Instance;

    void Awake()
    {
        Instance = this;
        MessagePanel.SetActive(false);
    }

    public void ShowMessage(string Player, string Phase, float Duration)
    {
        StartCoroutine(ShowMessageCoroutine(Player, Phase, Duration));
    }

    IEnumerator ShowMessageCoroutine(string Player, string Phase, float Duration)
    {
        MessageText.text = Player + " " + Phase + " phase";
        MessagePanel.SetActive(true);

        yield return new WaitForSeconds(Duration);

        MessagePanel.SetActive(false);
        Command.CommandExecutionComplete();
    }

}
