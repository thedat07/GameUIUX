using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class ButtonGame : ButtonBase, IPointerEnterHandler, IPointerExitHandler
{
    public enum TypeButton
    {
        None,
        Setting,
        Play,
        MoreLife,
        MoreCoin,
        Retry,
        Quit,
        Store,
        Home,
        NoAds,
        StartPack,
        Revive,
    }

    [Header("Type")]
    public TypeButton type = TypeButton.None;

    [Header("Audio")]
    public bool activeAudio = true;
    public TypeAudio typeAudio = TypeAudio.ButtonClick;

    [Header("Effect")]
    public bool activeEffect = true;

    [Header("Haptic")]
    public bool activeHaptic = true;

    private Sequence seq;

    private Vector3[] scales =
    {
        new Vector3(1f, 1f, 1f),
        new Vector3(1.2086f, 1.0963f, 1.0963f),
        new Vector3(0.95f, 1f, 1f),
        new Vector3(1f, 1f, 1f),
        new Vector3(0.985f, 1f, 1f),
        new Vector3(1f, 1f, 1f)
    };

    private float[] times =
    {
        0.08f,
        0.17f,
        0.17f,
        0.20f,
        0.31f
    };

    /// <summary>
    /// Phát âm thanh khi nhấn nút.
    /// </summary>
    protected override void PlayAudio()
    {
        if (activeAudio)
            GameManager.Instance
                       .GetSettingModelView()
                       .PlaySound(typeAudio);
    }

    /// <summary>
    /// Tạo hiệu ứng scale khi click nếu được bật.
    /// </summary>
    protected override void PlayEffect()
    {
        if (!activeEffect) return;

        if (this != null && transform != null && gameObject != null)
        {
            PlayPressedAnimation(scales, times);
        }

        if (!activeHaptic) return;

        GameManager.Instance.GetSettingModelView().TapSelectionHaptic();

        Log();
    }

    protected virtual void Log()
    {
        if (type != TypeButton.None)
        {
            if (FirebaseEventLogger.GetCategory() == FirebaseEventLogger.Category.Home)
            {
                FirebaseEventLogger.LogButtonClick(type.ToString().ToLower(), "Khi user click ở màn hình Home");
            }
            else
            {
                FirebaseEventLogger.LogButtonClick(type.ToString().ToLower(), "Khi user click ở màn hình InGame");
            }
        }
    }

    public void PlayPressedAnimation(Vector3[] keyScales, float[] durations)
    {
        if (keyScales == null || durations == null || keyScales.Length != durations.Length + 1)
        {
            Console.LogError("KeyScales phải nhiều hơn Durations đúng 1 phần tử!");
            return;
        }

        if (seq != null && seq.IsActive())
        {
            seq.Kill();
        }

        transform.localScale = Vector3.Scale(keyScales[0], m_Scale);

        seq = DOTween.Sequence();

        for (int i = 1; i < keyScales.Length; i++)
        {
            seq.Append(transform.DOScale(Vector3.Scale(keyScales[i], m_Scale), durations[i - 1]));
        }

        seq.SetEase(Ease.Linear);

        seq.SetLink(gameObject, LinkBehaviour.KillOnDestroy);
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);

        if (seq != null && seq.IsActive())
        {
            seq.Kill();
        }

        seq = DOTween.Sequence();

        transform.localScale = Vector3.Scale(scales[0], m_Scale);

        seq.Append(transform.DOScale(Vector3.Scale(Vector3.one * 1.1f, m_Scale), times[1]));

        seq.SetEase(Ease.Linear);

        seq.SetLink(gameObject, LinkBehaviour.KillOnDestroy);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);

        if (seq != null && seq.IsActive())
        {
            seq.Kill();
        }

        transform.localScale = Vector3.Scale(scales[0], m_Scale);
    }
}

#if UNITY_EDITOR
namespace Lean.Gui.Editor
{
    using UnityEditor;
    using TARGET = ButtonGame;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(TARGET))]
    public class ButtonGame_Editor : ButtonBase_Editor
    {
        protected override void DrawSelectableSettings()
        {
            base.DrawSelectableSettings();

            Draw("type", "Loại Button");

            Draw("activeAudio", "Bật/tắt am thanh.");

            Draw("typeAudio", "Loại âm thanh phát khi nhấn nút.");

            Draw("activeEffect", "Bật/tắt hiệu ứng scale khi click.");

            Draw("activeHaptic", "Bật/tắt rung.");
        }
    }
}
#endif