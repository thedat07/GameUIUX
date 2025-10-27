using UnityEngine;
using System.Collections.Generic;


public class ReviveOfferSpawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> reviveOffers;

    void Start()
    {
        foreach (var item in reviveOffers)
        {
            item.SetActive(false);
        }
        reviveOffers[GameManager.Instance.GetShopModelView().IDRevive].SetActive(true);
    }
}
