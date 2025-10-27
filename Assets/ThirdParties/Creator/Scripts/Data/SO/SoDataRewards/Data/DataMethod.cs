using System.Collections.Generic;
using UnityEngine.Events;

/// <summary>
/// Lớp dùng để chứa và xử lý phần thưởng của người chơi.
/// Có thể bao gồm nhiều vật phẩm (InventoryItem), log và callback.
/// </summary>
[System.Serializable]
public class DataMethod
{
    // Danh sách phần thưởng (vật phẩm và số lượng)
    public List<InventoryItem> data;


    public string title;

    // Log mô tả nguồn gốc của phần thưởng (dùng để tracking/debug)
    public string log;

    // Callback được gọi sau khi nhận phần thưởng xong
    public UnityAction callback;

    /// <summary>
    /// Khởi tạo DataRewards với danh sách phần thưởng, log và callback tùy chọn.
    /// </summary>
    /// <param name="data">Danh sách phần thưởng</param>
    /// <param name="log">Ghi chú hoặc nguồn phần thưởng</param>
    /// <param name="callback">Hàm được gọi sau khi xử lý xong</param>
    public DataMethod(List<InventoryItem> data, string title = "", string log = "", UnityAction callback = null)
    {
        // Tạo danh sách mới để tránh sửa trực tiếp dữ liệu gốc
        this.data = new List<InventoryItem>();
        this.data.AddRange(data);

        this.log = log;
        this.callback = callback;
        this.title = title;
    }

    public DataMethod(InventoryItem data, string title = "", string log = "", UnityAction callback = null)
    {
        // Tạo danh sách mới để tránh sửa trực tiếp dữ liệu gốc
        this.data = new List<InventoryItem>();
        this.data.Add(data);

        this.log = log;
        this.callback = callback;
        this.title = title;
    }

    /// <summary>
    /// Xử lý phần thưởng, áp dụng cho người chơi. Có thể nhân số lượng nếu cần.
    /// </summary>
    /// <param name="xData">Hệ số nhân cho phần thưởng (mặc định là 1)</param>
    public void Apply(int multiplier = 1, MasterModelView.TypeSource typeSource = MasterModelView.TypeSource.Free)
    {
        // Chuyển đổi dữ liệu phần thưởng (nếu có hệ thống mapping)
        List<InventoryItem> dataConvert = HelperCreator.Convert(data);

        if (dataConvert.Count > 0)
        {
            for (int i = 0; i < dataConvert.Count; i++)
            {
                // Nếu là phần thưởng "NoAds", thì kích hoạt tính năng tắt quảng cáo
                if (dataConvert[i].GetDataType() == MasterDataType.NoAds)
                {
                    GameManager.Instance.GetAdsModelView().OnRemoveAds();
                }
                else
                {
                    // Tính số lượng mới nếu có hệ số nhân
                    int newValue = dataConvert[i].GetQuantity() * multiplier;

                    // Gửi dữ liệu phần thưởng đến hệ thống quản lý tài nguyên (vàng, đá quý,...)
                    GameManager.Instance.GetMasterModelView().Post(newValue, dataConvert[i].GetDataType(), log, typeSource);
                }
            }

            // Phát hiệu ứng nhận thưởng (âm thanh, animation,...)
            Creator.Director.Object.PlayEffect();
        }

        // Gọi callback sau khi xử lý xong phần thưởng
        callback?.Invoke();
    }
}

/// <summary>
/// Sử dụng Builder Pattern để xây dựng đối tượng DataRewards.
/// Giúp việc thêm item, set log và callback linh hoạt và dễ đọc hơn.
/// </summary>
public class DataMethodBuilder
{
    // Danh sách các phần thưởng (vật phẩm + số lượng)
    private List<InventoryItem> items = new List<InventoryItem>();

    // Thông điệp log (hiển thị nguồn phần thưởng)
    private string log = "";

    private string title = "";

    // Callback được gọi sau khi nhận thưởng
    private UnityAction callback;

    /// <summary>
    /// Thêm một item vào danh sách phần thưởng từ ItemData và số lượng.
    /// </summary>
    public DataMethodBuilder AddItem(ItemData itemData, int quantity)
    {
        items.Add(new InventoryItem(itemData, quantity));
        return this; // Trả về chính builder để chain tiếp
    }

    /// <summary>
    /// Thêm một item có sẵn kiểu InventoryItem.
    /// </summary>
    public DataMethodBuilder AddItem(InventoryItem item)
    {
        items.Add(item);
        return this;
    }

    /// <summary>
    /// Thêm nhiều item một lúc (danh sách).
    /// </summary>
    public DataMethodBuilder AddItems(IEnumerable<InventoryItem> itemsToAdd)
    {
        items.AddRange(itemsToAdd);
        return this;
    }

    /// <summary>
    /// Gán thông điệp log cho phần thưởng.
    /// </summary>
    public DataMethodBuilder SetLog(string logMessage)
    {
        log = logMessage;
        return this;
    }

    /// <summary>
    /// Gán callback được gọi khi người chơi nhận thưởng xong.
    /// </summary>
    public DataMethodBuilder SetCallback(UnityAction cb)
    {
        callback = cb;
        return this;
    }

    /// <summary>
    /// Xây dựng và trả về đối tượng DataRewards với các thiết lập hiện tại.
    /// </summary>
    public DataMethod Build()
    {
        return new DataMethod(items, title, log, callback);
    }

    /// <summary>
    /*
        // Giả sử bạn có các dữ liệu vật phẩm (ItemData) lấy từ database hoặc config
        ItemData coinData = ShopPresenter.Instance.GetItem("coin");
        ItemData gemData = ShopPresenter.Instance.GetItem("gem");

        // Sử dụng builder để tạo phần thưởng một cách dễ đọc, rõ ràng
        DataMethod rewards = new DataMethodBuilder()
            .AddItem(coinData, 100)                          // Thêm 100 xu
            .AddItem(gemData, 10)                            // Thêm 10 đá quý
            .SetLog("Thưởng khi hoàn thành nhiệm vụ ngày")  // Ghi log cho tracking/analytics
            .SetCallback(() => Debug.Log("Đã nhận xong phần thưởng")) // Callback khi xử lý xong
            .Build();                                        // Tạo DataRewards từ các thiết lập trên

        // Thực thi nhận phần thưởng (áp dụng cho người chơi)
        rewards.Apply();
    */
    /// </summary>
}