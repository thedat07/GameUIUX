using System.Linq;
using Core;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Game/SoColorImage", order = 1)]
public class SoColorImage : ScriptableObject
{
    [System.Serializable]
    public class ColorImageData
    {
        public GameColor type;
        public Sprite sprite;
    }


    public ColorImageData[] colorImageDatas;

    public Sprite GetImage(GameColor type)
    {
        return colorImageDatas.FirstOrDefault(x => x.type == type).sprite;
    }
}
