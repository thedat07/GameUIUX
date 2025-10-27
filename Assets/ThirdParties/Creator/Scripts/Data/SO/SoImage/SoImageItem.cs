using UnityEngine;
using UnityEngine.UI;
using System.Linq;

[System.Serializable]
public class ImageItemData
{
    public MasterDataType type;
    public Sprite[] sprites;
}

[System.Serializable]
public class InfoImageView
{
    public Image image;

    public void View(SoImageItem soImage, InventoryItem data)
    {
        image.sprite = soImage.GetImage(data.GetDataType(), data.GetQuantity());
    }

}

[CreateAssetMenu(fileName = "Data", menuName = "Game/SoImageItem", order = 1)]
public class SoImageItem : ScriptableObject
{
    public ImageItemData[] datas;

    public Sprite GetImage(MasterDataType type, int vaule = 0)
    {
        try
        {
            var data = datas.First(x => x.type == type);
            if (type == MasterDataType.Money && datas.Length > 0)
            {
                if (vaule <= 50)
                {
                    return datas.First(x => x.type == type).sprites[0];
                }
                else if (vaule > 50 && vaule <= 150)
                {
                    return datas.First(x => x.type == type).sprites[1];
                }
                else
                {
                    return datas.First(x => x.type == type).sprites[2];
                }
            }
            else
            {
                return datas.First(x => x.type == type).sprites[0];
            }
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
            return null;
        }
    }
}
