using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class DailyRewardsAdsView : MonoBehaviour
{
    FeatureDailyRewradsAds GetData() => GameManager.Instance.GetFeatureData().featureDailyRewradsAds;

    [SerializeField] int m_Index;

    [Header("Info_1")]
    [SerializeField] GameObject[] m_Claim;
    [SerializeField] GameObject[] m_ClaimDone;
    [SerializeField] GameObject[] m_Lock;

    void Start()
    {
        View(GetData().GetData().claimIndex.Value);
        GetData().GetData().claimIndex.Subscribe(x => { View(x); }).AddTo(this);
    }

    public void View(int index)
    {
        SetActive(m_Claim, false);
        SetActive(m_ClaimDone, false);
        SetActive(m_Lock, false);

        if (m_Index < index)
            SetActive(m_ClaimDone, true);
        else if (m_Index == index)
            SetActive(m_Claim, true);
        else
            SetActive(m_Lock, true);
    }

    void SetActive(GameObject[] lst, bool active)
    {
        foreach (var item in lst)
        {
            item.SetActive(active);
        }
    }
}
