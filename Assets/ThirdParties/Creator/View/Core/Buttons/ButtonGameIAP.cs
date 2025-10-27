using UnityEngine.Events;
using UnityEngine;
using TMPro;
using Gley.EasyIAP;
using System.Collections.Generic;
using Gley.EasyIAP.Internal;
using System.Collections;

public class ButtonGameIAP : ButtonGame
{
    [Header("Setting")]
    public ShopProductNames yourPorduct;
    public TextMeshProUGUI textPrice;
    public InfoRewardViewRoot infoViewRoot;
    public UnityEvent OnUpdateView;

    [Header("Event")]
    public UnityEvent OnSuccess;
    public UnityEvent OnFail;
    public UnityEvent OnCompleted;

    protected List<InventoryItem> m_Data;

    [SerializeField] ProductType m_ProductType;

    protected override void StartButton()
    {
        Init();

        UpdateView();

        void Init()
        {
            m_Data = Gley.EasyIAP.API.GetValue(yourPorduct);

            m_ProductType = Gley.EasyIAP.API.GetProductType(yourPorduct);

            InfoRewardData infoReward = new InfoRewardData(m_Data);

            infoViewRoot.Initialize(infoReward);

            if (textPrice)
                textPrice.text = Gley.EasyIAP.API.GetLocalizedPriceString(yourPorduct).ToString();
        }

        if (m_ProductType == ProductType.NonConsumable || m_ProductType == ProductType.Subscription)
        {
            TigerForge.EventManager.StartListening(ShopModelView.Key, UpdateView);
        }
    }

    protected override void OnClickEvent()
    {
        GameManager.Instance.GetShopModelView().BuyProduct(yourPorduct, OnSuccessIAP, OnFailIAP, OnCompletedIAP);

        void OnCompletedIAP()
        {
            OnCompleted?.Invoke();
            if (m_ProductType == ProductType.NonConsumable || m_ProductType == ProductType.Subscription)
            {
                UpdateView();
            }
        }

        void OnSuccessIAP()
        {
            OnSuccess?.Invoke();
        }

        void OnFailIAP()
        {
            OnFail?.Invoke();
        }
    }

    protected override void UpdateButton()
    {
        if (IAPManager.Instance.IsInitialized() && textPrice && textPrice.text == "-")
        {
            textPrice.text = Gley.EasyIAP.API.GetLocalizedPriceString(yourPorduct).ToString();
        }
    }

    public virtual void UpdateView()
    {
        if (m_ProductType == ProductType.NonConsumable || m_ProductType == ProductType.Subscription)
        {
            bool canAdBeShown = Gley.MobileAds.API.CanShowAds();
            if (yourPorduct == ShopProductNames.RemoveAds)
            {
                interactable = canAdBeShown;
            }
            else if (m_ProductType == ProductType.Subscription)
            {
                bool isActive = !Gley.EasyIAP.API.IsActive(yourPorduct);
                interactable = isActive;
            }
            else
            {
                bool isActive = !GameManager.Instance.GetShopModelView().HasReceivedReward(yourPorduct.ToString());
                interactable = isActive;
            }
        }
        OnUpdateView?.Invoke();
    }

    protected override void DestroyButton()
    {
        base.DestroyButton();

        if (m_ProductType == ProductType.NonConsumable || m_ProductType == ProductType.Subscription)
        {
            TigerForge.EventManager.StopListening(ShopModelView.Key, UpdateView);
        }
    }

    protected override void Log()
    {
        if (FirebaseEventLogger.GetCategory() == FirebaseEventLogger.Category.Home)
        {
            FirebaseEventLogger.LogButtonClick(yourPorduct.ToString().ToLower(), "Khi user click IAP ở màn hình Home");
        }
        else
        {
            FirebaseEventLogger.LogButtonClick(yourPorduct.ToString().ToLower(), "Khi user click IAP ở màn hình InGame");
        }
    }
}

#if UNITY_EDITOR
namespace Lean.Gui.Editor
{
    using UnityEditor;
    using TARGET = ButtonGameIAP;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(TARGET))]
    public class ButtonGameIAP_Editor : ButtonGame_Editor
    {
        protected override void DrawSelectableSettings()
        {
            base.DrawSelectableSettings();

            Draw("yourPorduct", "Gói bán IAP được liên kết với nút.");

            Draw("textPrice", "Giá bán IAP");

            Draw("infoViewRoot", "UI hiển thị thông tin phần thưởng.");
        }

        protected override void DrawSelectableEvents(bool showUnusedEvents)
        {
            TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

            base.DrawSelectableEvents(showUnusedEvents);

            if (showUnusedEvents == true || Any(tgts, t => t.OnDown.GetPersistentEventCount() > 0))
            {
                Draw("OnSuccess", "Sự kiện được gọi khi người chơi mua thành công.");
            }

            if (showUnusedEvents == true || Any(tgts, t => t.OnClick.GetPersistentEventCount() > 0))
            {
                Draw("OnFail", "Sự kiện được gọi khi mua bị hủy hoặc lỗi.");
            }

            if (showUnusedEvents == true || Any(tgts, t => t.OnClick.GetPersistentEventCount() > 0))
            {
                Draw("OnCompleted", "Sự kiện được gọi khi mua kết thúc (bất kể thành công hay thất bại).");
            }
        }
    }
}
#endif