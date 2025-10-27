using System.Collections;
using System.Collections.Generic;
using Lean.Gui;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class RewardsItemRView : MonoBehaviour
{
    public LeanHover buttonGame;

    [Header("View")]
    public SpineAnimUIUXController icon;

    [Header("Rewards")]
    public RectTransform pointSpawn;

    private InventoryItem[] m_Data;
    private TheStickyCollectorController m_TSC;
    private GameObject m_View;

    void Start()
    {
        if (buttonGame)
        {
            buttonGame.OnEnter.AddListener(OnEnter);
            buttonGame.OnExit.AddListener(OnExit);
        }
    }

    public void Init(SoStoreRewards.RewardData data, TheStickyCollectorController theStickyCollector)
    {
        icon.GetSkeletonGraphic().Skeleton.SetSkin(data.indexIcon.ToString());
        icon.PlayIdleOnly(true);

        this.m_Data = data.rewards;
        this.m_TSC = theStickyCollector;
    }

    public void OnEnter()
    {
        if (m_View == null)
        {
            m_View = m_TSC.SpawnViewInfo(m_Data, pointSpawn);
            icon.PlayOpen();
        }
    }

    public void OnExit()
    {
        if (m_View != null)
        {
            Lean.Pool.LeanPool.Despawn(m_View);
            m_View = null;
            icon.PlayIdleOnly(true);
        }
    }

}
