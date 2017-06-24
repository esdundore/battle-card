using UnityEngine;
using UnityEngine.UI;

public class AvatarManager : MonoBehaviour {

    public AvatarAsset avatarAsset;
    public Image avatarImage;
    public Text gutsText;
    public Image CardFaceGlowImage;


    void Awake()
    {
        if (avatarAsset != null)
            ReadCardFromAsset();
    }

    public void ReadCardFromAsset()
    {
        // 1) change the avatar sprite
        avatarImage.sprite = avatarAsset.image;
        // 2) add guts amount
        gutsText.text = avatarAsset.gutsPoolAmount.ToString();

    }

}
