using System.Collections.Generic;
using UnityEngine;
using Coffee.UISoftMask;

public class InfoItemsView : MonoBehaviour
{
    public RectTransformFitter rectTransformFitter;

    [Header("Rewards")]
    public Transform pointSpawn;
    public RewardsItemView rewardsItemView;
    public List<RewardsItemView> itemViews;
    

    public void Init(InventoryItem[] data, RectTransform point)
    {
        rectTransformFitter.target = point;

        foreach (var item in itemViews)
        {
            Lean.Pool.LeanPool.Despawn(item);
        }

        itemViews = new List<RewardsItemView>();
        
        foreach (var item in data)
        {
            var reward = Lean.Pool.LeanPool.Spawn(rewardsItemView, pointSpawn);
            reward.Init(item);
            itemViews.Add(reward);
        }
    }
    
}
