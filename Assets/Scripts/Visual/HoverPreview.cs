using UnityEngine;
using DG.Tweening;

public class HoverPreview: MonoBehaviour
{
    // PUBLIC FIELDS
    public GameObject TurnThisOffWhenPreviewing;  // if this is null, will not turn off anything 
    public Vector3 TargetPosition;
    public float TargetScale;
    public GameObject previewGameObject;
    public static GameObject previewCard;
    public bool ActivateInAwake = false;

    // PRIVATE FIELDS
    private static HoverPreview currentlyViewing = null;

    // PROPERTIES WITH UNDERLYING PRIVATE FIELDS
    private static bool _PreviewsAllowed = true;
    public static bool PreviewsAllowed
    {
        get { return _PreviewsAllowed;}

        set 
        { 
            //Debug.Log("Hover Previews Allowed is now: " + value);
            _PreviewsAllowed= value;
            if (!_PreviewsAllowed)
                StopAllPreviews();
        }
    }

    private bool _thisPreviewEnabled = false;
    public bool ThisPreviewEnabled
    {
        get { return _thisPreviewEnabled;}

        set 
        { 
            _thisPreviewEnabled = value;
            if (!_thisPreviewEnabled)
                StopThisPreview();
        }
    }

    public bool OverCollider { get; set;}
 
    // MONOBEHVIOUR METHODS
    void Awake()
    {
        ThisPreviewEnabled = ActivateInAwake;
    }
            
    void OnMouseEnter()
    {
        OverCollider = true;
        if (PreviewsAllowed && ThisPreviewEnabled)
            PreviewThisObject();
    }
        
    void OnMouseExit()
    {
        OverCollider = false;

        if (!PreviewingSomeCard())
            StopAllPreviews();
    }

    // OTHER METHODS
    void PreviewThisObject()
    {
        // 1) clone this card 
        // first disable the previous preview if there is one already
        StopAllPreviews();
        // 2) save this HoverPreview as curent
        currentlyViewing = this;
        // 3) enable Preview game object
        previewGameObject.SetActive(true);
        // 4) disable if we have what to disable
        if (TurnThisOffWhenPreviewing!=null)
            TurnThisOffWhenPreviewing.SetActive(false); 
        // 5) tween to target position
        previewGameObject.transform.localPosition = Vector3.zero;
        previewGameObject.transform.localScale = Vector3.one;

        previewGameObject.transform.DOLocalMove(TargetPosition, 1f).SetEase(Ease.OutQuint);
        previewGameObject.transform.DOScale(TargetScale, 1f).SetEase(Ease.OutQuint);

        SkillCardManager skillCardManager = GetComponentInParent<SkillCardManager>();
        MonsterManager monsterManager = GetComponentInParent<MonsterManager>();
        if (skillCardManager != null)
        {
            previewCard = GameObject.Instantiate(GameStateSync.Instance.SkillCardPrefab,
                GameStateSync.Instance.previewCard.position,
                Quaternion.Euler(new Vector3(0f, 0f, 0f))) as GameObject;
            SkillCardManager manager = previewCard.GetComponent<SkillCardManager>();
            manager.cardAsset = GetComponentInParent<SkillCardManager>().cardAsset;
            manager.ReadCardFromAsset();
            Destroy(manager.GetComponent<HoverPreview>());
        }
        else if (monsterManager != null)
        {
            previewCard = GameObject.Instantiate(GameStateSync.Instance.MonsterCardPrefab,
                GameStateSync.Instance.previewCard.position,
                Quaternion.Euler(new Vector3(0f, 0f, 0f))) as GameObject;
            MonsterManager manager = previewCard.GetComponent<MonsterManager>();
            manager.monsterAsset = GetComponentInParent<MonsterManager>().monsterAsset;
            manager.ReadCardFromAsset();
            Destroy(manager.GetComponent<HoverPreview>());
        }
        previewCard.transform.DOScale(1.3f, 1f).SetEase(Ease.OutQuint);

    }

    void StopThisPreview()
    {
        previewGameObject.SetActive(false);
        previewGameObject.transform.localScale = Vector3.one;
        previewGameObject.transform.localPosition = Vector3.zero;
        if (TurnThisOffWhenPreviewing!=null)
            TurnThisOffWhenPreviewing.SetActive(true);
        Destroy(previewCard);
    }

    // STATIC METHODS
    private static void StopAllPreviews()
    {
        if (currentlyViewing != null)
        {
            currentlyViewing.StopThisPreview();
        }
         
    }

    private static bool PreviewingSomeCard()
    {
        if (!PreviewsAllowed)
            return false;

        HoverPreview[] allHoverBlowups = GameObject.FindObjectsOfType<HoverPreview>();

        foreach (HoverPreview hb in allHoverBlowups)
        {
            if (hb.OverCollider && hb.ThisPreviewEnabled)
                return true;
        }

        return false;
    }

   
}
