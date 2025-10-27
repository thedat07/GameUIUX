using UnityEngine;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
using UnityEditor;
#endif

public enum TypeAudio
{
    ButtonClick = -1,
    Coin,
    LevelCompleted,
    LevelFailed,
    IAP,
    Life,
    Booster1,
    Booster2,
    Booster3,
    BoosterClear,

    Block_Down,
    Block_Up,
    Char_Jump1,
    Char_Jump2,
    Char_Jump3,
    Hole_Close,
    CoinClaim,
    Spawn,
    Char_Jump4
}

[CreateAssetMenu(fileName = "Data", menuName = "Game/SoAudio", order = 1)]
public class SoAudio : SerializedScriptableObject
{
    [System.Serializable]
    public class Data
    {
        public TypeAudio type;
        public AudioClip clip;
    }

    public AudioClip click;

    public AudioClip sf;

    public AudioClip sf2;

    public Data[] vfx;
}