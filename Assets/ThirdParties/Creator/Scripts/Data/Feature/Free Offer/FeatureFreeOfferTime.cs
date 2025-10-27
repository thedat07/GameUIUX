using UnityEngine;
using UnityUtilities;
using System;

public partial class FeatureFreeOffer
{
    public bool IsDuringEvent()
    {
        return m_Data.featureDataEventTime.IsInEvent();
    }

    public bool IsAfterEvent()
    {
        return NetworkTime.UTC >= m_Data.featureDataEventTime.GetEventEndTime();
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