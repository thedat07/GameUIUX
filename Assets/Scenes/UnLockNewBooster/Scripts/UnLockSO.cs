using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/UnLockSO", order = 1)]
public class UnLockSO : ScriptableObject
{
    [System.Serializable]
    public class Data
    {
        public MasterDataType type;

        public string txtTile;

        [TextArea]
        public string txtTut;

        public Sprite icon;
    }

    public Data[] datas;

    public Data GetData(MasterDataType type)
    {
        return datas.FirstOrDefault(x => x.type == type);
    }
}
