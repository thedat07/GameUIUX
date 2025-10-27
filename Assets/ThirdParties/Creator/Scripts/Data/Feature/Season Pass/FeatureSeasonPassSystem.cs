using System;
using System.Collections.Generic;
using UnityEngine;
using UnityUtilities;

public partial class FeatureSeasonPass
{
    public void Claim(out int index)
    {
        index = -1;
        if (!IsActiveEvent()) return;
        index = m_Data.claimIndex.Value;
        m_Data.claimIndex.Value++;
        SaveData();
    }

    public void ClaimFree(int index)
    {
        if (!IsActiveEvent()) return;

        if (!m_Data.HasClaimFree(index))
        {
            m_Data.AddClaimFree(index);
            SaveData();
        }
    }

    public void ClaimVip(int index)
    {
        if (!IsActiveEvent()) return;
        if (!IsVip()) return;

        if (!m_Data.HasClaimVip(index))
        {
            m_Data.AddClaimVip(index);
            SaveData();
        }
    }

    public bool ActiveVip()
    {
        if (!IsActiveEvent()) return false;
        if (!IsVip())
        {
            m_Data.activeVip.Value = true;
            SaveData();
            return true;
        }
        return false;
    }

    public void AddExpOnWin(int exp)
    {
        if (!IsActiveEvent()) return;

        m_Data.exp.Value += exp;
        SaveData();
    }
}