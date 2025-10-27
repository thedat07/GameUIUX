using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// Dữ liệu mô tả một loại vật phẩm cơ bản (coin, gem, key, no-ads...).
/// Không chứa số lượng.
/// </summary>
[System.Serializable]
public class ItemData
{
    [ValueDropdown("@HelperCreator.ShopTypes")]
    public MasterDataType type;     // Loại vật phẩm (định danh bằng enum, ví dụ: Coin, Gem, NoAds...)
    public string displayName;      // Tên hiển thị lên UI (ví dụ: "Đá quý", "Vàng")
    public Sprite icon;             // Hình ảnh icon để hiển thị trong giao diện
}

/// <summary>
/// Đại diện cho một vật phẩm cụ thể kèm theo số lượng.
/// Ví dụ: 100 vàng, 5 đá quý.
/// </summary>
[System.Serializable]
public class InventoryItem
{
    [SerializeField] ItemData m_ItemData; // Thông tin vật phẩm cơ bản (loại, tên, icon)

    [Min(0)]
    [SerializeField] int m_Quantity;      // Số lượng vật phẩm đang có

    public InventoryItem()
    {
        m_ItemData = new ItemData();
        m_ItemData.type = MasterDataType.Money;
        m_ItemData.displayName = "";
        m_ItemData.icon = null;
        m_Quantity = 0;
    }

    /// <summary>
    /// Tạo vật phẩm với dữ liệu và số lượng ban đầu.
    /// </summary>
    public InventoryItem(ItemData data, int qty)
    {
        m_ItemData = data;
        m_Quantity = qty;
    }

    /// <summary>
    /// Lấy loại vật phẩm (để xử lý phần thưởng, hệ thống tiền tệ...)
    /// </summary>
    public MasterDataType GetDataType()
    {
        return m_ItemData != null ? m_ItemData.type : MasterDataType.Money;
    }

    /// <summary>
    /// Gán lại loại vật phẩm nếu cần thay đổi.
    /// </summary>
    public void SetDataType(MasterDataType type) => m_ItemData.type = type;

    /// <summary>
    /// Lấy icon hiển thị của vật phẩm.
    /// </summary>
    public Sprite GetIcon() => m_ItemData.icon;

    /// <summary>
    /// Lấy tên hiển thị của vật phẩm.
    /// </summary>
    public string GetName() => m_ItemData.displayName;

    /// <summary>
    /// Lấy số lượng vật phẩm đang có.
    /// </summary>
    public int GetQuantity() => m_Quantity;

    /// <summary>
    /// Gán lại số lượng mới cho vật phẩm.
    /// </summary>
    public void SetQuantity(int value) => m_Quantity = value;
}