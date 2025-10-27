using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UniRx;
using Core;

public static class HelperCreator
{
    public static MasterDataType[] BoosterTypes = new MasterDataType[] {
        MasterDataType.Booster1,
        MasterDataType.Booster2,
        MasterDataType.Booster3,
    };

    public static readonly MasterDataType[] ShopTypes = new[]
    {
        MasterDataType.Money,
        MasterDataType.Booster1,
        MasterDataType.Booster2,
        MasterDataType.Booster3,
        MasterDataType.NoAds,
        MasterDataType.Lives,
        MasterDataType.LivesInfinity,
        MasterDataType.X2Item,
        MasterDataType.None
    };

    public static readonly MasterDataType[] ShopQuantityTypes = new[]
    {
        MasterDataType.Money,
        MasterDataType.Booster1,
        MasterDataType.Booster2,
        MasterDataType.Booster3,
        MasterDataType.Lives,
        MasterDataType.LivesInfinity
    };

    public static readonly MasterDataType[] ShopQuantityTime = new[]
    {
        MasterDataType.LivesInfinity
    };


    public static void StopEverythingInScene()
    {
        // Stop all coroutines on all active MonoBehaviours
        MonoBehaviour[] allBehaviours = Object.FindObjectsOfType<MonoBehaviour>();
        foreach (var behaviour in allBehaviours)
        {
            behaviour.StopAllCoroutines();
        }

        // Kill all active DOTween tweens (also complete them if needed)
        DOTween.KillAll(); // Pass 'true' if you want to complete tweens before killing

        // Optional: reset time scale if it was changed
        Time.timeScale = 1f;

        // Optional: clear DOTween memory (pools, etc.)
        DOTween.Clear();
    }

    public static List<InventoryItem> Convert(List<InventoryItem> data)
    {
        if (data != null)
        {
            var groupedByType = data
             .GroupBy(item => item.GetDataType())
             .Select(group => new InventoryItem(
                new ItemData()
                {
                    type = group.Key
                },
                group.Sum(item => item.GetQuantity())
             ))
             .ToList();
            return groupedByType;
        }
        else
        {
            return new List<InventoryItem>();
        }
    }

    public static T JsonToObject<T>(string value)
    {
        return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(value);
    }

    public static string ObjectToJson<T>(T classT)
    {
        return Newtonsoft.Json.JsonConvert.SerializeObject(classT);
    }

    public static DG.Tweening.Sequence Register(float delay, UnityAction callBack, GameObject target = null, LinkBehaviour behaviour = LinkBehaviour.KillOnDestroy)
    {
        DG.Tweening.Sequence sequence = DOTween.Sequence();

        sequence.SetLink(target, behaviour);

        sequence.Append(DOVirtual.DelayedCall(delay, () => { callBack?.Invoke(); }));

        return sequence;
    }

    public static DG.Tweening.Sequence DOTweenSequence(GameObject target, LinkBehaviour behaviour = LinkBehaviour.KillOnDestroy)
    {
        DG.Tweening.Sequence sequence = DOTween.Sequence();
        sequence.SetLink(target, behaviour);
        return sequence;
    }

    public static bool IsNotNone(ItemData itemData)
    {
        return itemData != null && itemData.type != MasterDataType.None;
    }

    public static int GetType(GameGlobalConfig.LevelType levelType)
    {
        switch (levelType)
        {
            case GameGlobalConfig.LevelType.None:
                return 0;
            case GameGlobalConfig.LevelType.Hard:
                return 1;
            case GameGlobalConfig.LevelType.SuperHard:
                return 2;
        }
        return 0;
    }

    public static TypeAudio GetBoosterAdioType(MasterDataType type)
    {
        switch (type)
        {
            case MasterDataType.Booster1:
                return TypeAudio.Booster1;
            case MasterDataType.Booster2:
                return TypeAudio.Booster2;
            case MasterDataType.Booster3:
                return TypeAudio.Booster3;
        }
        return 0;
    }

    private static readonly GameColor[] brightColors =
{
         GameColor.Blue ,
         GameColor.Green ,
         GameColor.Red ,
         GameColor.Yellow ,
         GameColor.Orange ,
         GameColor.Pink ,
         GameColor.Purple ,
         GameColor.DarkBlue ,
         GameColor.DarkGreen ,
         GameColor.LightBlue ,
    };

    public static GameColor GetRandomBrightColor()
    {
        return brightColors[UnityEngine.Random.Range(0, brightColors.Length)];
    }

    public static Color GetColor(GameColor color)
    {
        switch (color)
        {
            case GameColor.Blue: return HexToColor("68B9F6");
            case GameColor.Green: return HexToColor("69F554");
            case GameColor.Red: return HexToColor("E85037");
            case GameColor.Yellow: return HexToColor("F4F546");
            case GameColor.Orange: return HexToColor("F97A37");
            case GameColor.Pink: return HexToColor("EB57AC");
            case GameColor.Purple: return HexToColor("8115E3");
            case GameColor.DarkBlue: return HexToColor("4A6CE8");
            case GameColor.DarkGreen: return HexToColor("359510");
            case GameColor.LightBlue: return HexToColor("B1EAFA");
            default: return Color.white;
        }
    }

    private static Color HexToColor(string hex)
    {
        Color color;
        if (ColorUtility.TryParseHtmlString("#" + hex, out color))
            return color;
        return Color.white;
    }
}
