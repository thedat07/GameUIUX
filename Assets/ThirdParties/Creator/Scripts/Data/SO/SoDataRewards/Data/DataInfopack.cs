using System.Collections.Generic;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;
using YNL.Utilities.Extensions;
using UnityUtilities;

[System.Serializable]
public class InfoTextView
{
    public TextMeshProUGUI text;
    public bool abbrevation = false;
    public bool isPlus = false;

    public void View(InventoryItem itemShop)
    {
        if (text == false) return;
        if (itemShop.GetQuantity() <= 0)
        {
            text.gameObject.SetActive(false);
        }
        else
        {
            if (itemShop.GetDataType() == MasterDataType.Money)
            {
                if (abbrevation)
                {
                    text.text = string.Format("{0}", itemShop.GetQuantity().ToAbbreviatedString());
                }
                else
                {
                    text.text = string.Format("{0}", itemShop.GetQuantity());
                }

            }
            else if (itemShop.GetDataType() == MasterDataType.LivesInfinity)
            {
                text.UpdateCountdownText(System.TimeSpan.FromMinutes((double)itemShop.GetQuantity()));
            }
            else
            {
                if (isPlus)
                {
                    text.text = string.Format("+{0}", itemShop.GetQuantity());
                }
                else
                {
                    text.text = string.Format("x{0}", itemShop.GetQuantity());
                }
            }
        }
    }
}


[System.Serializable]
public class InfoIconView
{
    public Image icon;
    public bool resize = false;

    public void View(InventoryItem itemShop)
    {
        if (itemShop.GetIcon())
        {
            icon.sprite = itemShop.GetIcon();
            if (resize)
            {
                icon.ReSize();
            }
        }
    }
}