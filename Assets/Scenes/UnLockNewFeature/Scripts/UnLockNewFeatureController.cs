using Creator;
using TMPro;
using UnityEngine.UI;

public class UnLockNewFeatureData
{
    public SoNewFeature.Data data;

    public UnLockNewFeatureData(SoNewFeature.Data data)
    {
        this.data = data;
    }
}

public class UnLockNewFeatureController : Controller
{
    public const string UNLOCKNEWFEATURE_SCENE_NAME = "UnLockNewFeature";

    public override string SceneName()
    {
        return UNLOCKNEWFEATURE_SCENE_NAME;
    }

    public TextMeshProUGUI txtDes;
    public TextMeshProUGUI txtName;
    public Image icon;

    public override void OnActive(object data)
    {
        if (data != null)
        {
            UnLockNewFeatureData featureData = data as UnLockNewFeatureData;
            txtDes.text = featureData.data.txtDes.ToString();
            txtName.text = featureData.data.txtName.ToString();
            icon.sprite = featureData.data.iconUnLock;
            FirebaseEventLogger.LogFeatureUnLock(featureData.data.txtName.ToString());
        }
    }
}