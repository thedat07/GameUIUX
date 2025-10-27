using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class Reward
{
    public int rewardId;   // tên/ID phần thưởng
    [Range(1, 100)]
    public int weight = 1;    // tỉ lệ rơi (trọng số)
    public bool isBigReward;  // đánh dấu phần thưởng "to"
}

public class SpinWheelController : MonoBehaviour
{
    [Header("Wheel Settings")]
    public Transform wheel;
    public float spinDuration = 4f;
    public int spinRounds = 5;

    [Header("Rewards")]
    public List<Reward> rewards;

    [Header("Guarantee Settings")]
    public int guaranteeAfterXSpins = 10; // sau X vòng auto ra big reward

    [Header("UI")]
    public CanvasGroup canvasGroup;

    public Action<Reward, MasterModelView.TypeSource> OnSpinEnd;

    private bool isSpinning = false;

    private FeatureLukySpin m_Data;

    [Header("So")]
    public SoStoreRewards soStoreRewards;

    int m_Id;

    public void Init(FeatureLukySpin data)
    {
        m_Data = data;


        OnSpinEnd += OnRewards;
    }

    public void StartSpin(MasterModelView.TypeSource type)
    {
        if (isSpinning) return;

        isSpinning = true;

        m_Data.UpdateSpinCount(false);

        int resultIndex;

        // Check guarantee
        if (m_Data.SpinCount() >= guaranteeAfterXSpins)
        {
            // chọn 1 reward "to"
            List<int> bigIndexes = new List<int>();
            for (int i = 0; i < rewards.Count; i++)
                if (rewards[i].isBigReward)
                    bigIndexes.Add(i);

            if (bigIndexes.Count > 0)
            {
                resultIndex = bigIndexes[UnityEngine.Random.Range(0, bigIndexes.Count)];
                m_Data.UpdateSpinCount(true);
            }
            else
            {
                // fallback nếu không có big reward -> random bình thường
                resultIndex = GetWeightedRandomIndex();
            }
        }
        else
        {
            // random theo tỉ lệ bình thường
            resultIndex = GetWeightedRandomIndex();
        }

        int sectorCount = rewards.Count;
        float sectorAngle = 360f / sectorCount;
        float targetAngle = resultIndex * sectorAngle;

        float totalRotation = (spinRounds * 360f) + targetAngle;

        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.5f;

        wheel.DOLocalRotate(
            new Vector3(0, 0, totalRotation),
            spinDuration,
            RotateMode.FastBeyond360
        )
        .SetEase(Ease.OutQuart)
        .OnComplete(() =>
        {
            canvasGroup.blocksRaycasts = true;
            isSpinning = false;
            canvasGroup.alpha = 1f;

            Reward result = rewards[resultIndex];

            m_Id = resultIndex;

            OnSpinEnd?.Invoke(result, type);

            UnityEngine.Console.Log($"Spin result: {result.rewardId} | Count = {m_Data.SpinCount()}");
        });
    }

    private int GetWeightedRandomIndex()
    {
        int totalWeight = 0;
        foreach (var r in rewards) totalWeight += r.weight;

        int randomValue = UnityEngine.Random.Range(0, totalWeight);
        int sum = 0;

        for (int i = 0; i < rewards.Count; i++)
        {
            sum += rewards[i].weight;
            if (randomValue < sum)
                return i;
        }

        return 0;
    }

    private void OnRewards(Reward data, MasterModelView.TypeSource type = MasterModelView.TypeSource.Free)
    {
        var dataReward = soStoreRewards.datas[data.rewardId];
        DataMethod r = new DataMethod(HelperCreator.Convert(dataReward.rewards.ToList()), "Spin Reward", StaticLogData.LogSpinRewards(m_Id));
        r.Apply(typeSource: type);
        Creator.ManagerDirector.PushScene(RewardsController.REWARDS_SCENE_NAME, r);
    }
}
