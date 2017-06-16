using UnityEngine;
using UnityEngine.UI;

public class MonsterManager : MonoBehaviour {

    public MonsterAsset monsterAsset;
    public MonsterManager PreviewManager;
    [Header("Text Component References")]
    public Text TitleText;
    public Text TypeText;
    public Text LifeText;
    [Header("Image References")]
    public Image CardBodyImage;
    public Image CardTitleImage;
    public Image CardTypeImage;
    public Image CardLifeImage;
    public Image CardFaceGlowImage;

    public int index;

    void Awake()
    {
        if (monsterAsset != null)
            ReadCardFromAsset();
    }

    public void ReadCardFromAsset()
    {
        // 1) change the card graphic sprite
        CardBodyImage.sprite = monsterAsset.image;
        // 2) add max life
        LifeText.text = monsterAsset.maxLife.ToString();

        if (PreviewManager != null)
        {
            PreviewManager.monsterAsset = monsterAsset;
            PreviewManager.ReadCardFromAsset();
        }
    }

}
