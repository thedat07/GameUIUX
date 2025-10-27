using UnityEngine;
using UnityUtilities;
using System;

public partial class FeatureTheStickyCollector
{
    public bool IsAfterEvent()
    {
        return NetworkTime.UTC > DateTime.Parse(m_Data.currentWeekEnd.Value);
    }

    public bool IsDuringEvent()
    {
        return NetworkTime.UTC >= DateTime.Parse(m_Data.currentWeekStart.Value) && NetworkTime.UTC <= DateTime.Parse(m_Data.currentWeekEnd.Value);
    }

    public string GetTime()
    {
        DateTime dateEnd = DateTime.Parse(m_Data.currentWeekEnd.Value);
        TimeSpan remainingTimeSpan = dateEnd - NetworkTime.UTC;
        TimeSpan timerToShow = remainingTimeSpan;
        string formatD = "{0}d {1}h"; string formatH = "{0}h {1}m";
        string formatM = "{0}m {1}s"; string formatZero = "--";
        return UnityUtilities.CountdownTextUtilities.FormatCountdownLives(timerToShow, formatD, formatH, formatM, formatZero);
    }

    public void CheckTimeReset()
    {
        DateTime now = NetworkTime.UTC;
        DateTime currentWeekEnd = DateTime.Parse(m_Data.currentWeekEnd.Value);

        if (now > currentWeekEnd)
        {
            DeleteData();
        }
    }

    public void CustomUpdate()
    {
        CheckTimeReset();
    }
}