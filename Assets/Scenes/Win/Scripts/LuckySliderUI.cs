using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine.Events;
using TMPro;

public class LuckySliderUI : MonoBehaviour
{
    [SerializeField] private Slider slider;         // Slider UI (min = 0, max = 1)
    [SerializeField] private List<float> rewards;  // Danh sách kết quả
    [SerializeField] private List<TextMeshProUGUI> lstItems;

    private Tween moveTween;
    private bool isSpinning = false;

    public TextMeshProUGUI txtFree;

    public TextMeshProUGUI txtAds;

    void Start()
    {
        // Đảm bảo slider setup đúng
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.wholeNumbers = false;

        StartSpinLoop();

        txtFree.text = StaticData.CoinWin.ToString();
    }

    public void StartSpinLoop()
    {
        if (moveTween != null) moveTween.Kill();

        slider.value = 0;

        // Tween chạy qua lại từ 0 -> 1 liên tục
        moveTween = DOTween.To(() => slider.value, x => slider.value = x, 1f, 1.5f)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo)
            .OnUpdate(UpdateHighlight)
            .SetLink(gameObject, LinkBehaviour.KillOnDestroy);

        isSpinning = true;
    }

    public void StopSpin(UnityAction<float> xRewards)
    {
        if (!isSpinning) return;
        isSpinning = false;

        if (moveTween != null) moveTween.Kill();

        // Tính step mỗi reward
        float step = 1f / (rewards.Count - 1);

        // Tìm index gần nhất
        int nearestIndex = Mathf.RoundToInt(slider.value / step);
        nearestIndex = Mathf.Clamp(nearestIndex, 0, rewards.Count - 1);

        float targetValue = nearestIndex * step;

        // Snap slider về vị trí reward gần nhất
        DOTween.To(() => slider.value, x => slider.value = x, targetValue, 0.3f)
            .SetEase(Ease.OutCubic)
            .OnComplete(() =>
            {
                xRewards?.Invoke(rewards[nearestIndex]);
            }).SetLink(gameObject, LinkBehaviour.KillOnDestroy);
    }

    private void UpdateHighlight()
    {
        float step = 1f / (rewards.Count - 1);
        int nearestIndex = Mathf.RoundToInt(slider.value / step);
        HighlightItem(nearestIndex);
    }

    private void HighlightItem(int index)
    {
        for (int i = 0; i < lstItems.Count; i++)
        {
            if (i == index)
            {
                int coinX = (int)(StaticData.CoinWin * rewards[index]);
                txtAds.text = coinX.ToString();
                float scale = rewards[index] * 0.9f;
                lstItems[i].transform.DOScale(scale, 0.25f).SetEase(Ease.Flash).SetLink(gameObject, LinkBehaviour.KillOnDestroy);
                lstItems[i].DOFade(1, 0.25f).SetEase(Ease.Flash).SetLink(gameObject, LinkBehaviour.KillOnDestroy);
            }
            else
            {
                lstItems[i].transform.DOScale(1f, 0.25f).SetEase(Ease.Flash).SetLink(gameObject, LinkBehaviour.KillOnDestroy);
                lstItems[i].DOFade(0.5f, 0.25f).SetEase(Ease.Flash).SetLink(gameObject, LinkBehaviour.KillOnDestroy);
            }
        }
    }
}
