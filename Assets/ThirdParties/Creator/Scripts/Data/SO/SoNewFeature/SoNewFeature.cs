using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Game/SoNewFeature", order = 1)]
public class SoNewFeature : ScriptableObject
{
    [System.Serializable]
    public class Data
    {
        public int requireLevel;
        public Sprite iconLock;
        public Sprite iconUnLock;

        public string txtName;

        [TextArea]
        public string txtDes;
    }

    public Data[] datas;
}
