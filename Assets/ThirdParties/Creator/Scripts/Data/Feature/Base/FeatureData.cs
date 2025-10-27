

using System;
using UnityUtilities;

public interface IFeatureUpdatable
{
    void CustomUpdate();
}

public class FeatureData
{
    // Level yêu cầu để mở khóa nhiệm vụ
    protected int m_LevelUnlock = 0;

    // Kiểu nhiệm vụ
    protected TypeFeature m_Type;

    // Constructor
    public FeatureData(TypeFeature type, int levelUnlock = 0)
    {
        this.m_Type = type;
        this.m_LevelUnlock = levelUnlock;
    }

    /// <summary>
    /// Kiểm tra điều kiện mở khóa nhiệm vụ (so sánh stage hiện tại)
    /// </summary>
    protected virtual bool IsUnlock()
    {
        return GameManager.Instance.GetMasterData().dataStage.Get() >= m_LevelUnlock;
    }

    /// <summary>
    /// GET: Lấy level yêu cầu mở khóa
    /// </summary>
    public int GetUnlockLevel() => m_LevelUnlock;

    /// <summary>
    /// PUT: Cập nhật level yêu cầu mở khóa
    /// </summary>
    public void SetUnlockLevel(int level) => m_LevelUnlock = level;

    /// <summary>
    /// GET: Loại nhiệm vụ
    /// </summary>
    public TypeFeature GetFeatureType() => m_Type;
}

public class FeatureDataRemoteConfig : FeatureData
{
    public bool IsRemoteConfigLoaded() => RemoteConfigController.IsInitSuccess;

    public FeatureDataRemoteConfig(TypeFeature type, int levelUnlock = 0) : base(type, levelUnlock) { }

    protected override bool IsUnlock()
    {
        return GameManager.Instance.GetMasterData().dataStage.Get() >= m_LevelUnlock && IsRemoteConfigLoaded();
    }
}

public class FeatureDataEventTime
{
    private int m_EventDuration;

    private int m_EventInterval;

    public FeatureDataEventTime(int eventDuration, int eventInterval)
    {
        this.m_EventDuration = eventDuration;
        this.m_EventInterval = eventInterval;
    }

    public DateTime GetEventStartTime()
    {
        DateTime now = NetworkTime.UTC;
        return now.GetEventStartTime(m_EventDuration, m_EventInterval);
    }

    public DateTime GetEventEndTime()
    {
        DateTime now = NetworkTime.UTC;
        return now.GetEventEndTime(m_EventDuration, m_EventInterval);
    }

    public bool IsInEvent()
    {
        DateTime now = NetworkTime.UTC;
        return now.IsInEvent(m_EventDuration, m_EventInterval);
    }

    public DateTime GetNextEventStartTime()
    {
        DateTime now = NetworkTime.UTC;
        return now.GetNextEventStartTime(m_EventDuration, m_EventInterval);
    }
}