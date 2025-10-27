using UnityUtilities;
using UniRx;
using System.Collections.Generic;
using System;

public partial class FeatureWinStreak
{
    public void Won()
    {
        if (IsDuringEvent())
        {
            int win = m_Data.currentStreak.Value + 1;
            m_Data.currentStreak.Value = win;
            m_Data.currentStreakClaim.Value = win;
            SaveData();
        }
    }

    public void Reset()
    {
        m_Data.Reset();
        SaveData();
    }

    public bool CanClaim(int index)
    {
        bool enoughStreak = m_Data.currentStreak.Value >= index;
        bool notClaimed = !m_Data.HasIdClaim(index);
        return enoughStreak && notClaimed && IsDuringEvent();
    }

    public bool Claim(int index)
    {
        if (CanClaim(index) && IsDuringEvent())
        {
            m_Data.AddIdClaim(index);
            SaveData();
            return true;
        }
        return false;
    }
}