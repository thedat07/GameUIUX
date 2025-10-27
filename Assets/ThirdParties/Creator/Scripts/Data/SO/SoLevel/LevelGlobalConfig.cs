using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using TW.Utility.Extension;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace GameGlobalConfig
{
    [CreateAssetMenu(fileName = "LevelGlobalConfig", menuName = "GlobalConfigs/LevelGlobalConfig")]
    [GlobalConfig("Assets/Resources/GlobalConfig/")]
    public class LevelGlobalConfig : GlobalConfig<LevelGlobalConfig>
    {
        [field: SerializeField] public int LevelMax { get; set; } = 2;
        [field: SerializeField] public int LevelMaxInConfig { get; set; }
        [field: SerializeField] public List<LevelConfig> LevelConfigs { get; set; } = new();

#if UNITY_EDITOR
        //[Button]
        private async UniTask FetchLevelData()
        {
            EditorUtility.SetDirty(this);
            LevelConfigs.Clear();
            for (int i = 0; i < LevelMaxInConfig; i++)
            {
                AssetReference levelPrefabReference = null;
                try
                {
                    levelPrefabReference =
                        new AssetReference(AssetDatabase.FindAssets($"t:Textasset Level_{i + 1}")[0]);
                }
                catch (Exception)
                {
                    // ignored
                }

                LevelConfig levelConfig = new LevelConfig
                {
                    level = i + 1,
                    levelType = LevelType.Normal,
                    levelPrefabReference = levelPrefabReference,
                };
                LevelConfigs.Add(levelConfig);
            }

            LevelMaxInConfig = LevelConfigs.Where(cf => cf.levelPrefabReference != null).Max(cf => cf.level);
            foreach (LevelConfig levelConfig in LevelConfigs)
            {
                if (levelConfig.levelPrefabReference == null)
                {
                    if (levelConfig.level <= LevelMax)
                    {
                        Debug.LogError($"Level {levelConfig.level} prefab reference is null, please check the config.");
                    }
                }
            }
        }

        public void UpdateLevelConfig(int level, LevelType levelType)
        {
            LevelConfig levelConfig = new();
            if (LevelConfigs.Any(lc => lc.level == level))
            {
                levelConfig = LevelConfigs.First(lc => lc.level == level);
                levelConfig.levelType = levelType;
                levelConfig.levelPrefabReference = new AssetReference(AssetDatabase.FindAssets($"t:Textasset Level_{level}")[0]); ;
            }
            else
            {
                levelConfig = new LevelConfig
                {
                    level = level,
                    levelType = levelType,
                    levelPrefabReference = new AssetReference(AssetDatabase.FindAssets($"t:Textasset Level_{level}")[0]),
                };
                LevelConfigs.Add(levelConfig);
            }
            EditorUtility.SetDirty(this);
        }
#endif
        [Button]
        public LevelConfig GetLevelConfig(int level)
        {
            // if (level < 1 || level > LevelMaxInConfig)
            // {
            //     return GetExtraLevelConfig(level);
            //     ;
            // }

            for (int i = 0; i < LevelConfigs.Count; i++)
            {
                if (LevelConfigs[i].level == level)
                {
                    return LevelConfigs[i];
                }
            }

            return GetRandomLevelConfig();
        }

        [Button]
        public LevelConfig GetExtraLevelConfig(int level)
        {
            int extraLevel = level;
            int oldLevel = XorShift32(extraLevel) % (LevelMax - 50) + 50;
            LevelConfig oldLevelConfig = GetLevelConfig(oldLevel);
            LevelConfig extraLevelConfig = oldLevelConfig.CreateExtra(extraLevel);
            return extraLevelConfig;
        }

        public int XorShift32(int seed)
        {
            seed ^= (seed << 13);
            seed ^= (seed >> 17);
            seed ^= (seed << 5);
            return seed & 0x7FFFFFFF; // Ensure non-negative
        }
        public LevelConfig GetRandomLevelConfig()
        {
            int randomLevel = UnityEngine.Random.Range(1, LevelMax + 1);
            return GetLevelConfig(randomLevel);
        }
    }

    [System.Serializable]
    public class LevelConfig
    {
        public int level;
        public LevelType levelType;
        public AssetReference levelPrefabReference;

        [ShowInInspector, ReadOnly] private float timeRemote;

        public void SetTimeRemote(float value)
        {
            timeRemote = value;
        }

        public LevelConfig CreateExtra(int extraLevel)
        {
            LevelConfig extraConfig = new LevelConfig
            {
                level = extraLevel,
                levelType = this.levelType,
                levelPrefabReference = this.levelPrefabReference,
                timeRemote = this.timeRemote
            };
            return extraConfig;
        }
    }

    public enum LevelType
    {
        None = 0,
        Normal = 1,
        Hard = 2,
        SuperHard = 3,
    }
}