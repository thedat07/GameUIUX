using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityUtilities;
using UnityTimer;

public class DailyRewardsFreeView : MonoBehaviour
{
    FeatureDailyRewradsFree GetData() => GameManager.Instance.GetFeatureData().featureDailyRewradsFree;

    [SerializeField] TextMeshProUGUI m_TxtTime;
    [SerializeField] GameObject m_Claim;
    [SerializeField] GameObject m_Time;

    private Timer m_UpdateTimer;

    private void OnEnable()
    {
        // Update UI ngay lúc mở
        UpdateUI();

        // Set timer chạy mỗi giây
        m_UpdateTimer = UnityTimer.Timer.Register(1f,
            onComplete: () => UpdateUI(),
            isLooped: true,
            autoDestroyOwner: this
        );
    }

    private void OnDisable()
    {
        if (m_UpdateTimer != null)
        {
            m_UpdateTimer.Cancel();
            m_UpdateTimer = null;
        }
    }

    private void UpdateUI()
    {
        bool canClaim = GetData().CanClaim();

        m_Claim.SetActive(canClaim);
        m_Time.SetActive(!canClaim);

        if (!canClaim)
        {
            int remainSeconds = (int)GetData().GetRemainingTime().TotalSeconds;
            m_TxtTime.text = DateTimeExtensions.ToTextTime(remainSeconds);
        }
    }
}
