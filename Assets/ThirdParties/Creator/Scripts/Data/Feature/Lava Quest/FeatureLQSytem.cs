using System;
using System.Collections.Generic;
using UnityEngine;
using UnityUtilities;

public partial class FeatureLavaQuest
{
    public void Won()
    {
        if (IsDuringEvent())
        {
            int win = m_Data.currentStreak.Value + 1;
            m_Data.currentStreak.Value = win;
            SaveData();
        }
    }

    public void Lose()
    {
        m_Data.SetInterval();
        SaveData();
    }

    public bool Claim()
    {
        if (IsDuringEvent())
        {
            m_Data.SetInterval();
            SaveData();
            return true;
        }
        return false;
    }
}