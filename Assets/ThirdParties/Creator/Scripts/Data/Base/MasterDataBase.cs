using UnityEngine;
using UniRx;
using UnityUtilities;
using System.Collections.Generic;

[System.Serializable]
public class MasterDataBase<T>
{
    // Giá trị mặc định
    protected T m_DefaultValue;

    // Kiểu dữ liệu Master (dùng làm key để lưu và truy xuất dữ liệu)
    public MasterDataType m_Type;

    // ReactiveProperty để theo dõi sự thay đổi
    private ReactiveProperty<T> _value;

    // Chỉ cho phép đọc từ bên ngoài
    public IReadOnlyReactiveProperty<T> Value => _value;

    /// <summary>
    /// Constructor khởi tạo dữ liệu
    /// </summary>
    /// <param name="defaultValue">Giá trị mặc định nếu chưa có dữ liệu lưu</param>
    /// <param name="type">Kiểu dữ liệu master</param>
    public MasterDataBase(T defaultValue, MasterDataType type)
    {
        this.m_Type = type;
        this.m_DefaultValue = defaultValue;

        // Lấy giá trị đã lưu (nếu có), nếu chưa thì dùng default
        T savedValue = SaveExtensions.GetMaster(m_Type, "value", defaultValue);

        // Gán vào reactive property
        _value = new ReactiveProperty<T>(savedValue);
    }

    /// <summary>
    /// Lấy giá trị hiện tại (getter thông thường)
    /// </summary>
    public T Get() => _value.Value;

    /// <summary>
    /// PUT: Gán giá trị mới trực tiếp
    /// </summary>
    public virtual void Put(T newValue)
    {
        // Lưu vào hệ thống lưu trữ
        SaveExtensions.PutMaster(m_Type, "value", newValue);

        // Gán cho ReactiveProperty => notify observer
        _value.Value = newValue;
    }

    /// <summary>
    /// DELETE: Reset giá trị về default
    /// </summary>
    public virtual void Delete()
    {
        SaveExtensions.PutMaster(m_Type, "value", m_DefaultValue);
        _value.Value = m_DefaultValue;
    }

    /// <summary>
    /// Lấy kiểu dữ liệu master
    /// </summary>
    public MasterDataType GetDataType() => m_Type;

    /// <summary>
    /// Lấy giá trị mặc định
    /// </summary>
    public T GetDefaultValue() => m_DefaultValue;
}

public class IntMasterData : MasterDataBase<int>
{
    public IntMasterData(int defaultValue, MasterDataType type)
        : base(defaultValue, type) { }

    public void Post(int amount)
    {
        int newValue = Mathf.Clamp(Get() + amount, 0, int.MaxValue);
        Put(newValue);
    }
}

public class ListIntMasterData : MasterDataBase<List<int>>
{
    public ListIntMasterData(List<int> defaultValue, MasterDataType type) 
        : base(defaultValue, type) { }

    public virtual void Post(int item)
    {
        var current = Get();
        current.Add(item);
        Put(current); 
    }

    public void Remove(int item)
    {
        var current = Get();
        if (current.Remove(item))
        {
            Put(current);
        }
    }

    public override void Delete()
    {
        Put(new List<int>(GetDefaultValue())); 
    }
}