using Creator;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YNL.Utilities.Extensions;
using BrunoMikoski.AnimationSequencer;
using Coffee.UISoftMask;

public class UnLockNewBoosterData
{
    public MasterDataType type;

    public Transform point;

    public Image icon;

    public UnLockNewBoosterData(MasterDataType type, Transform point, Image icon)
    {
        this.type = type;
        this.point = point;
        this.icon = icon;
    }
}

public class UnLockNewBoosterController : Controller
{
    public const string UNLOCKNEWBOOSTER_SCENE_NAME = "UnLockNewBooster";

    public override string SceneName()
    {
        return UNLOCKNEWBOOSTER_SCENE_NAME;
    }

    [Header("Ref")]
    public UnLockSO unLockSO;

    [Header("View")]
    public Image icon;
    public Image iconMask;
    public TextMeshProUGUI txtName;
    public TextMeshProUGUI txtDes;
    public RectTransform maskingShape;

    [Header("View Mask")]
    public Image imageView;
    public SpineAnimUIUXController animHand;
    public AnimationSequencerController anim;

    private UnLockNewBoosterData m_Data;
    private UnLockSO.Data m_DataType;

    public override void OnActive(object data)
    {
        if (data != null)
        {
            m_Data = data as UnLockNewBoosterData;
            m_DataType = unLockSO.GetData(m_Data.type);
            InitView();
            View();
        }
    }

    void InitView()
    {
        txtName.text = string.Format("{0}", m_DataType.txtTile);
        
        txtDes.text = string.Format("{0}", m_DataType.txtTut);

        if (icon)
        {
            icon.sprite = m_DataType.icon;
        }

        if (m_Data.point)
        {
            maskingShape.transform.position = m_Data.point.position;
            if (m_Data.icon)
            {
                iconMask.sprite = m_Data.icon.sprite;
                iconMask.ReSize();
                imageView.sprite = m_Data.icon.sprite;
                imageView.ReSize();
            }
        }
        
        animHand.PlayIdleHand();
    }

    public void OnClaim()
    {
        Animation.shield.gameObject.SetActive(false);
        anim.Play();
    }

    void View()
    {
        switch (m_Data.type)
        {
            case MasterDataType.Booster1:
                ViewBooster1();
                break;
            case MasterDataType.Booster2:
                ViewBooster2();
                break;
            case MasterDataType.Booster3:
                ViewBooster3();
                break;
        }
    }

    void ViewBooster1()
    {

    }

    void ViewBooster2()
    {

    }

    void ViewBooster3()
    {

    }
}