using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Gley.EasyIAP;
using UnityTimer;
using Creator;
using DG.Tweening;

public class ShopModelView : MonoBehaviour, IInitializable
{
    public static string Key = "AdsModelView";

    [SerializeField] GameObject m_ShieldShop;

    [System.Serializable]
    public class ReviveOfferData
    {
        public int ID;   // Prefab/UI của gói Revive Offer
        public int weight = 1;      // Trọng số (mặc định = 1)
    }
    [SerializeField] private List<ReviveOfferData> reviveOffers;

    public int IDRevive;

    public void Initialize()
    {
        Gley.EasyIAP.API.Initialize(InitializationComplete);

        void InitializationComplete(IAPOperationStatus status, string message)
        {
            if (status == IAPOperationStatus.Success)
            {
                ShopProductNames[] shopProductNames = new ShopProductNames[] {
                    ShopProductNames.RemoveAds,
                    ShopProductNames.RemoveAdsBundle
                };

                foreach (var item in shopProductNames)
                {
                    if (API.IsActive(item))
                    {
                        if (API.GetValue(item).Any(x => x.GetDataType() == MasterDataType.NoAds))
                        {
                            GameManager.Instance.GetAdsModelView().OnRemoveAds();
                        }
                    }
                }
            }
            else
            {
                Console.Log("IAP", "Error occurred: " + message);
            }
        }

        IDRevive = SpawnRandomReviveOffer();
    }


    public int SpawnRandomReviveOffer()
    {
        if (reviveOffers == null || reviveOffers.Count == 0)
        {
            return 0;
        }

        int totalWeight = 0;
        foreach (var offer in reviveOffers)
            totalWeight += offer.weight;

        int randomValue = Random.Range(0, totalWeight);
        int current = 0;

        foreach (var offer in reviveOffers)
        {
            current += offer.weight;
            if (randomValue < current)
            {
                return offer.ID;
            }
        }

        return 0;
    }

    public void BuyProduct(ShopProductNames shopProduct, UnityAction onSuccess = null, UnityAction onFail = null, UnityAction onCompleted = null, string log = "")
    {
        SetActiveShield(true);

        Gley.EasyIAP.API.BuyProduct(shopProduct, ProductBought);

        void ProductBought(IAPOperationStatus status, string message, StoreProduct product)
        {
            if (status == IAPOperationStatus.Success)
            {
                ProductType productType = Gley.EasyIAP.API.GetProductType(shopProduct);
                if (productType == ProductType.Consumable)
                {
                    TypeConsumable();
                }
                else
                {
                    TypeNonConsumable();
                }

                UpdateIapCount();

                FirebaseEventLogger.LogPurchaseClick(shopProduct.ToString(), 1);

                TigerForge.EventManager.EmitEvent(Key);
            }
            else
            {
                onFail?.Invoke();

                FirebaseEventLogger.LogPurchaseClick(shopProduct.ToString(), 0);
            }

            onCompleted?.Invoke();

            SetActiveShield(false);

            void TypeConsumable()
            {
                SetVaule(product.value, "CONGRATULATIONS", shopProduct.ToString());
                onSuccess?.Invoke();
            }

            void TypeNonConsumable()
            {
                string productId = product.productName;
                if (!HasReceivedReward(productId))
                {
                    TypeConsumable();
                    MarkRewarded(productId);
                }
                else
                {
                    onFail?.Invoke();
                }
            }
        }
    }

    public void OnRestore()
    {
        SetActiveShield(true);
        Gley.EasyIAP.API.RestorePurchases(ProductRestoredCallback, RestoreDone);
    }

    // automatically called after one product is restored, is the same as the Buy Product callback
    private void ProductRestoredCallback(IAPOperationStatus status, string message, StoreProduct product)
    {
        if (status == IAPOperationStatus.Success)
        {
            ShopProductNames[] shopProductNames = new ShopProductNames[] {
                ShopProductNames.RemoveAds,
                ShopProductNames.RemoveAdsBundle
            };

            foreach (var item in shopProductNames)
            {
                ShopProductNames productName = API.ConvertNameToShopProduct(product.productName);
                if (item == productName)
                {
                    string productId = product.productName;
                    if (!HasReceivedReward(productId))
                    {
                        SetVaule(product.value, "Restore", "restore");
                        MarkRewarded(productId);
                    }
                }
            }
            //consumable products are not restored
        }
        else
        {
            //an error occurred in the buying process, log the message for more details
            Console.Log("IAP", "Restore product failed: " + message);
        }
    }

    private void RestoreDone()
    {
        Console.Log("IAP", "Restore done");
        SetActiveShield(false);
        TigerForge.EventManager.EmitEvent(Key);
    }

    void SetVaule(List<InventoryItem> data, string title = "", string log = "")
    {
        if (data.Count > 0 && data != null)
        {
            DataMethod r = new DataMethod(HelperCreator.Convert(data), title, log);
            r.Apply(typeSource: MasterModelView.TypeSource.Iap);
            ManagerDirector.PushScene(RewardsController.REWARDS_SCENE_NAME, r);
        }
    }

    public bool HasReceivedReward(string productId)
    {
        return ES3.Load<bool>("IAP_Rewarded_" + productId, "IAPData", defaultValue: false);
    }

    private void MarkRewarded(string productId)
    {
        ES3.Save("IAP_Rewarded_" + productId, true, "IAPData");
        StaticDataFeature.RefreshFeatureActive();
    }

    private Tween m_AutoOffTween;

    private Tween m_DeactivateTween;

    void SetActiveShield(bool active)
    {
        if (active)
        {
            m_ShieldShop.gameObject.SetActive(true);

            m_AutoOffTween?.Kill();
            m_DeactivateTween?.Kill();


            m_AutoOffTween = DOVirtual.DelayedCall(1f, () =>
            {
                if (m_ShieldShop.gameObject.activeSelf)
                {
                    SetActiveShield(false);
                }
            }).SetLink(gameObject);
        }
        else
        {
            m_AutoOffTween?.Kill();
            m_AutoOffTween = null;

            m_DeactivateTween?.Kill();
            m_DeactivateTween = DOVirtual.DelayedCall(StaticData.DelayTimeDefault, () =>
            {
                m_ShieldShop.gameObject.SetActive(false);
            }).SetLink(gameObject);
        }
    }

    public void UpdateIapCount()
    {
        StaticData.IAPCount += 1;
        FirebaseUserProperties.SetIapCount(StaticData.IAPCount);
    }
}
