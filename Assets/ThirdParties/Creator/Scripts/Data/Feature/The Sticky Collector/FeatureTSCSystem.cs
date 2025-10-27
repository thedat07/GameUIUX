using System;
using System.Collections.Generic;
using UnityEngine;
using UnityUtilities;

public partial class FeatureTheStickyCollector
{
    public void Post(int amount)
    {
        if (StaticDataFeature.ActiveEventTSC())
        {
            int newValue = Mathf.Clamp(m_Data.amount.Value + amount, 0, Int16.MaxValue);
            Put(newValue);
        }
    }

    private void Put(int value)
    {
        m_Data.amount.Value = Mathf.Clamp(value, 0, Int16.MaxValue);
        SaveData();
    }

    public void Claim(int indexClaim)
    {
        m_Data.claimIndex.Value = indexClaim;
        SaveData();
    }
}