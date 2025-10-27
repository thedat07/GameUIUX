using UnityEngine;
using Lean.Gui;

/// <summary>
/// Lớp cơ bản mở rộng từ LeanButton.
/// Cung cấp khung xử lý sự kiện cho các nút có thêm hiệu ứng, âm thanh, logic riêng.
/// </summary>
public class ButtonBase : LeanButton
{
    protected Vector3 m_Scale;

    /// <summary>
    /// Gọi khi object được khởi tạo. Dùng để lưu scale ban đầu và chạy logic khởi tạo riêng.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

        if (!Application.isPlaying) return;

        if (transform.localScale == Vector3.zero)
        {
            m_Scale = Vector3.one;
        }
        else
        {
            m_Scale = transform.localScale;
        }

        AwakeButton();
    }

    /// <summary>
    /// Gọi sau Awake. Gắn các listener cho sự kiện click.
    /// </summary>
    protected override void Start()
    {
        base.Start();

        if (!Application.isPlaying) return;

        OnClick.AddListener(PlayAudio);
        OnClick.AddListener(PlayEffect);
        OnClick.AddListener(OnClickEvent);
        OnClick.AddListener(OnClickLogEvent);
        StartButton();
    }

    /// <summary>
    /// Gọi mỗi frame. Dùng để cập nhật trạng thái của nút.
    /// </summary>
    protected virtual void Update()
    {
        if (!Application.isPlaying) return;

        UpdateButton();
    }

    /// <summary>
    /// Gọi khi object bị hủy. Dùng để giải phóng tài nguyên hoặc hủy đăng ký sự kiện.
    /// </summary>
    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (!Application.isPlaying) return;

        DestroyButton();
    }

    /// <summary>
    /// Override để thêm hiệu ứng âm thanh khi click nút.
    /// </summary>
    protected virtual void PlayAudio() { }

    /// <summary>
    /// Override để thêm hiệu ứng hình ảnh khi click nút.
    /// </summary>
    protected virtual void PlayEffect() { }

    /// <summary>
    /// Override để xử lý logic khi click nút.
    /// </summary>
    protected virtual void OnClickEvent() { }

    /// <summary>
    /// Override để xử lý logic khi click nút.
    /// </summary>
    protected virtual void OnClickLogEvent() { }

    /// <summary>
    /// Override để thêm xử lý riêng trong Awake().
    /// </summary>
    protected virtual void AwakeButton() { }

    /// <summary>
    /// Override để thêm xử lý riêng trong Start().
    /// </summary>
    protected virtual void StartButton() { }

    /// <summary>
    /// Override để thêm xử lý riêng trong Update().
    /// </summary>
    protected virtual void UpdateButton() { }

    /// <summary>
    /// Override để thêm xử lý riêng khi object bị destroy.
    /// </summary>
    protected virtual void DestroyButton() { }
}

#if UNITY_EDITOR
namespace Lean.Gui.Editor
{
    using UnityEditor;
    using TARGET = ButtonBase;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(TARGET))]
    public class ButtonBase_Editor : LeanButton_Editor
    {

    }
}
#endif