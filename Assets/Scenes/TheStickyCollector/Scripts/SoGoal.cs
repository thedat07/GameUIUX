using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Game/SoGoal", order = 1)]
public class SoGoal : SoStoreRewards
{
    public int[] goals;

    public struct ProgressData
    {
        public int current;
        public int goal;
        public int index;
        public bool isMax;

        public ProgressData(int current, int goal, int index, bool isMax)
        {
            this.current = current;
            this.goal = goal;
            this.index = index;
            this.isMax = isMax;
        }

        public override string ToString()
        {
            return isMax ? "MAX" : $"{current}/{goal}";
        }

        public string GetIndex() => $"{index + 1}";

        public int GetIndexClaim()
        {
            int plusMax = isMax ? 1 : 0;
            return index + plusMax;
        }

        public float GetProgress()
        {
            if (!isMax)
            {
                if (goal <= 0) return 0f;
                return (float)current / goal;
            }
            return 1;
        }
    }

    public List<InventoryItem> GetData(int from, int to)
    {
        List<InventoryItem> items = new List<InventoryItem>();

        for (int i = from; i < to; i++)
        {
            items.AddRange(GetRewardData(i));
        }

        return items;
    }

    public ProgressData GetProgress(int totalCollected)
    {
        int sum = 0;
        for (int i = 0; i < goals.Length; i++)
        {
            int goal = goals[i];
            int nextSum = sum + goal;

            // Nếu tổng đã vượt qua hoặc vừa đạt mốc này
            if (totalCollected < nextSum)
            {
                int current = totalCollected - sum;
                bool isMax = false;
                return new ProgressData(current, goal, i, isMax);
            }

            sum = nextSum;
        }

        return new ProgressData(0, 0, goals.Length - 1, true);
    }
}
