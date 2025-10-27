using Sirenix.OdinInspector;
using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public partial class GameManager
{
    private float m_Interval = 5f;
    private long m_TimeOnline = 0;
    private Coroutine m_Coroutine;

    private IEnumerator OnlineLoop()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(m_Interval);

            if (!(m_TimeOnline > long.MaxValue - (long)m_Interval))
            {
                m_TimeOnline += (long)m_Interval;
                FirebaseUserProperties.SetOnlineTime(m_TimeOnline);
            }
        }
    }

    private void OnDestroy()
    {
        if (m_Coroutine != null)
            StopCoroutine(m_Coroutine);
    }

    [Button]
    public void RunPlay(int retry = 0)
    {
        if (m_MasterModelView.CanPlay())
        {
            bool isRandomColor = StaticData.IsRandomColor;

            int level = GameManager.Instance.GetStageModelView().GetLoopLevel();

            Creator.Director.RunScene(GamePlayController.GAMEPLAY_SCENE_NAME, new GamePlaySceneData(level, isRandomColor, retry));

            StaticData.IsRandomColor = true;
        }
    }

    public void PlayNext(bool win = true)
    {
        int level = GameManager.Instance.GetMasterData().dataStage.Get();
        if (StaticData.LevelBackHome <= 0)
        {
            RunPlay();
        }
        else
        {
            if (level < StaticData.LevelBackHome)
            {
                RunPlay();
            }
            else
            {
                Creator.Director.RunScene(HomeController.HOME_SCENE_NAME, new HomeData(win));
            }
        }
    }

    public void RunHome()
    {
        Creator.Director.RunScene(HomeController.HOME_SCENE_NAME);
    }

    public void RegisterNextFrame(UnityAction callback)
    {
        GameManager.Instance.StartCoroutine(NextFrame(callback));
    }

    private IEnumerator NextFrame(UnityAction callback)
    {
        yield return null;
        callback?.Invoke();
    }
}
