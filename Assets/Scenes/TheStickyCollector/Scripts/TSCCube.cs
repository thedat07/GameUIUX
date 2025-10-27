using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class TSCCube : MonoBehaviour
{
    public SoColorImage soColorImage;

    public Image icon;

    public FeatureTheStickyCollector GetDataTSC() => GameManager.Instance.GetFeatureData().featureTSC;

    void Start()
    {
        if (StaticDataFeature.ActiveEventTSC())
        {
            icon.sprite = soColorImage.GetImage(GetDataTSC().GetColor());
        }
    }

}
