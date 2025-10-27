using UnityEngine;
using UnityUtilities;
using System;

public partial class FeatureLavaQuest
{
    public bool IsDuringEvent()
    {
        return NetworkTime.UTC < m_Data.EventEndTime;
    }

    public bool IsAfterEvent()
    {
        return NetworkTime.UTC >= m_Data.GetTimeCooldownEvent();
    }

    public void CheckTimeReset()
    {
        if (!m_Data.IsReset())
            return;

        if (IsAfterEvent())
        {
            DeleteData();
        }
    }

    public void CustomUpdate()
    {
        CheckTimeReset();
    }
}