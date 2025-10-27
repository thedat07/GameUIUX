using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Game/SoStoreRewards", order = 1)]
public class SoStoreRewards : ScriptableObject
{
    [System.Serializable]
    public class RewardData
    {
        public Sprite iconRewards;
        public int indexIcon = -1;
        public InventoryItem[] rewards;

        public RewardData()
        {
            rewards = new InventoryItem[0];
        }
    }

    public RewardData[] datas;

    public InventoryItem[] GetRewardData(int index)
    {
        if (index < datas.Length)
        {
            return datas[index].rewards;
        }
        return new InventoryItem[0];
    }

    public RewardData GetReward(int index)
    {
        if (index < datas.Length)
        {
            return datas[index];
        }
        return new RewardData();
    }
}
