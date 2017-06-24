using UnityEngine;
using UnityEngine.UI;

public class SkillCardManager : MonoBehaviour {

    public CardAsset cardAsset;
    public SkillCardManager PreviewManager;
    [Header("Text Component References")]
    public Text GutsText;
    public Text TitleText;
    public Text UserText;
    public Text DescriptionText;
    public Text TypeText;
    public Text DamageText;
    [Header("Image References")]
    public Image CardBodyImage;
    public Image CardGutsImage;
    public Image CardTitleImage;
    public Image CardGraphicImage;
    public Image CardUserImage;
    public Image CardTypeImage;
    public Image CardDamageImage;
    public Image CardFaceGlowImage;

    void Awake()
    {
        if (cardAsset != null)
            ReadCardFromAsset();
    }

    public void ReadCardFromAsset()
    {
        // 1) apply tint
        if (cardAsset.monsterAsset != null)
        {
            CardBodyImage.color = cardAsset.monsterAsset.classCardTint;
            CardGutsImage.color = cardAsset.monsterAsset.classRibbonsTint;
            CardTitleImage.color = cardAsset.monsterAsset.classRibbonsTint;
            CardUserImage.color = cardAsset.monsterAsset.classRibbonsTint;
            CardTypeImage.color = cardAsset.monsterAsset.classRibbonsTint;
            CardDamageImage.color = cardAsset.monsterAsset.classRibbonsTint;
        }
        // 2) add guts cost
        GutsText.text = cardAsset.gutsCost.ToString();
        // 3) add card title
        TitleText.text = cardAsset.name;
        // 4) change the card graphic sprite
        CardGraphicImage.sprite = cardAsset.image;
        // 5) add user
        UserText.text = cardAsset.user;
        // 6) add description
        DescriptionText.text = cardAsset.description;
        // 6) add type
        TypeText.text = cardAsset.type;
        // 7) add damage
        DamageText.text = cardAsset.damage.ToString();
        if (cardAsset.damage == 0)
        {
            CardDamageImage.enabled = false;
            DamageText.enabled = false;
        }

        if (PreviewManager != null)
        {
            PreviewManager.cardAsset = cardAsset;
            PreviewManager.ReadCardFromAsset();
        }
    }

}
