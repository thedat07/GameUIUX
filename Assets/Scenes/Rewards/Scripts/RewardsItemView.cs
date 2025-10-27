using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardsItemView : MonoBehaviour
{
    public InventoryItem info;

    public SoImageItem soData;

    public InfoImageView infoImageView;

    public InfoTextView txtInfo;

    public void Init(InventoryItem data)
    {
        this.info = data;

        if (infoImageView.image)
            infoImageView.View(soData, data);

        if (txtInfo.text)
            txtInfo.View(data);
    }

    public void Init(InventoryItem data, Sprite sprite)
    {
        this.info = data;
        
        if (infoImageView.image)
            infoImageView.image.sprite = sprite;

        if (txtInfo.text)
            txtInfo.View(data);
    }
}
