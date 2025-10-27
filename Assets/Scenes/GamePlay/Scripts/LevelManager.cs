using DesignPatterns;
using Core.Events;
using UniRx;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    public ReactiveProperty<bool> IsPause = new ReactiveProperty<bool>(false);

    private float m_LevelDuration { get; set; }
    private float m_MaxLevelDuration { get; set; }
    private float m_MaxFreezeDuration { get; set; }
    private float m_FreezeDuration { get; set; }

    void Start()
    {
        GameEventSub.OnLevelLose.Subscribe(_ => { OnLevelLose(); }).AddTo(this);
        GameEventSub.OnLevelWin.Subscribe(_ => { OnLevelWin(); }).AddTo(this);
        GameEventSub.CheckWinCondition.Subscribe(_ => { CheckWinCondition(); }).AddTo(this);
    }

    public void SetPause(bool isPause)
    {
        IsPause.Value = isPause;
    }

    private void Update()
    {
        if (IsPause.Value) return;

        if (m_LevelDuration <= 0) return;

        if (m_FreezeDuration > 0)
        {
            m_FreezeDuration -= Time.deltaTime;
        }
        else
        {
            m_LevelDuration -= Time.deltaTime;
        }

        (float a, float b, float c) dataTimeFreeze = new(m_FreezeDuration, m_MaxFreezeDuration, Time.deltaTime);
        GameEventSub.OnFreezeTimeChange.OnNext(dataTimeFreeze);

        (float a, float b, float c) dataTimeInGame = new(m_LevelDuration, m_MaxLevelDuration, Time.deltaTime);
        GameEventSub.OnTimeInGameChange.OnNext(dataTimeInGame);

        if (m_LevelDuration > 0) return;
        m_LevelDuration = 0;
        GameEventSub.OnLevelLose.OnNext(Unit.Default);
    }

    #region WinLose
    private void OnLevelWin()
    {
        SetPause(true);
    }

    private void OnLevelLose()
    {
        SetPause(true);
    }

    public void TryAddTime(float duration)
    {
        m_LevelDuration += duration;

        (float a, float b, float c) dataTimeInGame = new(m_LevelDuration, m_MaxLevelDuration, Time.deltaTime);
        GameEventSub.OnTimeInGameChange.OnNext(dataTimeInGame);
    }

    private void CheckWinCondition()
    {
        if (IsWin())
        {
            GameEventSub.OnLevelWin.OnNext(Unit.Default);
        }

        bool IsWin()
        {
            return false;
        }
    }
    #endregion

    #region Freeze
    public void AddFreezeDuration(float time)
    {
        m_MaxFreezeDuration = time;
        m_FreezeDuration = time;

        (float a, float b, float c) dataTimeFreeze = new(m_FreezeDuration, m_MaxFreezeDuration, Time.deltaTime);
        GameEventSub.OnFreezeTimeChange.OnNext(dataTimeFreeze);
    }

    public bool CanUseBoosterFreeze()
    {
        return m_FreezeDuration <= 0;
    }
    #endregion
}
