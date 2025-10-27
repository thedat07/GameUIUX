using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

/// <summary>
/// Hiển thị dữ liệu một loại tài nguyên cụ thể (Coin, Gem,...).
/// </summary>
[System.Serializable]
public class InfoViewData
{
    [ValueDropdown("@HelperCreator.ShopTypes")]
    public MasterDataType type;

    public GameObject content;

    [Header("View")]
    public InfoTextView textView;
    public InfoIconView iconView;

    public void Show(InventoryItem item)
    {
        if (content)
            content.SetActive(true);

        textView.View(item);
        iconView.View(item);
    }

    public void Hide()
    {
        if (content)
            content.SetActive(false);
    }
}

/// <summary>
/// Dữ liệu đầu vào cho hệ thống hiển thị phần thưởng.
/// </summary>
public class InfoRewardData
{
    public List<InventoryItem> items;

    public InfoRewardData(List<InventoryItem> items)
    {
        this.items = items ?? new List<InventoryItem>();
    }
}

/// <summary>
/// Gắn vào GameObject trong scene, giữ tham chiếu đến view data.
/// </summary>
public class InfoView : MonoBehaviour
{
    public InfoViewData infoViewData;
}

/// <summary>
/// Thành phần quản lý và cập nhật các InfoView theo phần thưởng người chơi nhận.
/// </summary>
[System.Serializable]
public class InfoRewardViewRoot : IInitializableData<InfoRewardData>
{
    public InfoView[] infoViews;

    // Tối ưu truy cập bằng từ điển
    private Dictionary<MasterDataType, InfoViewData> viewDataMap;

    /// <summary>
    /// Khởi tạo cấu trúc ánh xạ nếu chưa có.
    /// </summary>
    private void SetupViews()
    {
        if (viewDataMap != null) return;

        viewDataMap = new Dictionary<MasterDataType, InfoViewData>();

        foreach (var view in infoViews)
        {
            if (view != null && view.infoViewData != null && !viewDataMap.ContainsKey(view.infoViewData.type))
            {
                viewDataMap[view.infoViewData.type] = view.infoViewData;
            }
        }
    }

    /// <summary>
    /// Khởi tạo giao diện hiển thị dữ liệu phần thưởng.
    /// </summary>
    public void Initialize(InfoRewardData inputData)
    {
        SetupViews();

        InfoRewardData rewardData = inputData;

        if (rewardData != null)
        {
            UpdateViews(rewardData.items);
        }
    }


    /// <summary>
    /// Cập nhật view từ danh sách phần thưởng.
    /// </summary>
    private void UpdateViews(List<InventoryItem> items)
    {
        HideAllViews();

        foreach (var item in items)
        {
            ShowMatchingItem(item);
        }
    }

    /// <summary>
    /// Ẩn toàn bộ view hiện tại.
    /// </summary>
    private void HideAllViews()
    {
        foreach (var viewData in viewDataMap.Values)
        {
            viewData.Hide();
        }
    }

    /// <summary>
    /// Hiển thị item nếu có view tương ứng.
    /// </summary>
    private void ShowMatchingItem(InventoryItem item)
    {
        var type = item.GetDataType();
        if (viewDataMap.TryGetValue(type, out var viewData))
        {
            viewData.Show(item);
        }
    }
}
