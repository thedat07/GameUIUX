using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Cơ sở dữ liệu chứa danh sách tất cả các vật phẩm trong game.
/// Sử dụng Singleton để dễ truy cập.
/// </summary>
[CreateAssetMenu(menuName = "Game/ItemDatabase")]
public class ItemDatabase : ScriptableObject
{
    [SerializeField] private List<ItemData> itemList = new List<ItemData>();

    // Dictionary để tra cứu nhanh theo loại vật phẩm
    private Dictionary<MasterDataType, ItemData> itemMap;

    /// <summary>
    /// Tự động xây dựng lại Dictionary khi ScriptableObject được kích hoạt.
    /// </summary>
    private void OnEnable()
    {
        BuildMap();
    }

    /// <summary>
    /// Xây dựng lại Dictionary tra cứu.
    /// </summary>
    private void BuildMap()
    {
        itemMap = new Dictionary<MasterDataType, ItemData>();

        foreach (var item in itemList)
        {
            if (item == null) continue;

            if (!itemMap.ContainsKey(item.type))
            {
                itemMap[item.type] = item;
            }
            else
            {
                Debug.LogWarning($"ItemDatabase đã chứa item với type {item.type}, bỏ qua item trùng.");
            }
        }
    }

    /// <summary>
    /// Lấy ItemData theo MasterDataType.
    /// </summary>
    public ItemData GetItem(MasterDataType type)
    {
        if (itemMap == null || itemMap.Count == 0)
        {
            BuildMap();
        }

        if (itemMap.TryGetValue(type, out var item))
        {
            return item;
        }

        Debug.LogWarning($"Không tìm thấy item với type: {type}");
        return null;
    }

    /// <summary>
    /// Trả về danh sách tất cả các item để hiển thị UI hoặc debug.
    /// </summary>
    public List<ItemData> GetAllItems()
    {
        return itemList;
    }
}