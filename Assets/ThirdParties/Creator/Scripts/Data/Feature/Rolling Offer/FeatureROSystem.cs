using System;
using System.Collections.Generic;
using UnityEngine;
using UnityUtilities;

public partial class FeatureRollingOffer
{
    public void Claim(out int index)
    {
        index = -1;
        if (!IsActiveEvent()) return;
        index = m_Data.claimIndex.Value;
        m_Data.claimIndex.Value++;
        SaveData();
    }
}