using System.Collections.Generic;
using UnityEngine;
using GameGlobalConfig;

[System.Serializable]
public class StageFirebase
{
    public List<int> levelGame;

    public StageFirebase()
    {
        levelGame = new List<int>();
    }
}


public class StageModelView : MonoBehaviour, IInitializable
{
    private MasterModel m_Model;

    public void Initialize()
    {
        m_Model = GameManager.Instance.GetMasterData();
    }

    public LevelGlobalConfig levelGlobalConfig;

    public int GetLoopLevel(int level)
    {
        int levelMax = levelGlobalConfig.LevelMax;

        // Nếu chưa vượt quá max thì giữ nguyên
        if (level <= levelMax)
            return level;

        // Xác định midpoint
        int midLevel = (1 + levelMax) / 2;

        // Khoảng loop = từ midLevel đến levelMax
        int loopRange = levelMax - midLevel + 1;

        // Tính offset
        int offset = (level - midLevel) % loopRange;

        return midLevel + offset;
    }

    public int GetTypeLevel(int level)
    {
        var data = levelGlobalConfig.GetLevelConfig(level);
        return HelperCreator.GetType(data.levelType);
    }

    public int GetLoopLevel()
    {
        return GetLoopLevel(GameManager.Instance.GetMasterData().dataStage.Get());
    }

    public int GetTypeLevel()
    {
        return GetTypeLevel(GetLoopLevel());
    }

    public int GetTypeLevelLoop(int plus)
    {
        return GetTypeLevel(GetLoopLevel() + plus);
    }
}
