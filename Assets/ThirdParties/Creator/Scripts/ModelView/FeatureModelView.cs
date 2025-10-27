using System.Collections.Generic;
using Core;
using UnityEngine;
using UnityTimer;
using System.Collections;
using Unity.Mathematics;

public class FeatureModelView : MonoBehaviour, IInitializable
{
    private FeatureModel m_Model;

    public Dictionary<TypeFeature, int> dataWin = new Dictionary<TypeFeature, int>();

    public void Initialize()
    {
        m_Model = GameManager.Instance.GetFeatureData();
    }

    private void Start()
    {
        StartCoroutine(UpdateEverySecond());
    }

    public void ClearDataWin()
    {
        dataWin = new Dictionary<TypeFeature, int>();
    }

    private IEnumerator UpdateEverySecond()
    {
        while (true)
        {
            m_Model.CustomUpdate();
            yield return new WaitForSeconds(0.5f);
        }
    }
    
    public void PostGameColor(TypeFeature type, Dictionary<GameColor, int> data)
    {
        if (StaticDataFeature.FeatureUnlockLevels.ContainsKey(type) &&
            StaticDataFeature.FeatureActive.TryGetValue(type, out bool isActive) && isActive &&
            m_Model.GetData(type) != null)
        {
            int levelUnlock = StaticDataFeature.FeatureUnlockLevels[type];

            if (m_Model.IsUnlock(levelUnlock))
            {
                int x2 = GameManager.Instance.GetMasterModelView().IsX2Item() ? 2 : 1;

                switch (type)
                {
                    case TypeFeature.TheStickyCollector:
                        {
                            GameColor color = m_Model.featureTSC.GetData().currentColor.Value;
                            if (data.ContainsKey(color))
                            {
                                int amount = data[color] * x2;
                                m_Model.featureTSC.Post(amount);
                                dataWin.Add(TypeFeature.TheStickyCollector, amount);
                            }
                        }
                        break;
                }
            }
        }
    }

    public void PostWon(TypeFeature type)
    {
        if (StaticDataFeature.FeatureUnlockLevels.ContainsKey(type) &&
            StaticDataFeature.FeatureActive.TryGetValue(type, out bool isActive) && isActive &&
            m_Model.GetData(type) != null)
        {
            int levelUnlock = StaticDataFeature.FeatureUnlockLevels[type];

            if (m_Model.IsUnlock(levelUnlock))
            {
                switch (type)
                {
                    case TypeFeature.PiggyBank:
                        m_Model.featurePiggyBank.AddExpOnWin();
                        break;
                }
            }
        }
    }
}