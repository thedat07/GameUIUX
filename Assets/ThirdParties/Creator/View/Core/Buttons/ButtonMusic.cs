using UnityEngine;
using UnityEngine.Events;

public class ButtonMusic : ButtonGame
{
    [Header("Event")]
    public UnityEvent On;

    public UnityEvent Off;

    protected override void StartButton()
    {
        UpdateView();
    }

    protected override void OnClickEvent()
    {
        GameManager.Instance.GetSettingModelView().ToggleMusic();
        UpdateView();
    }

    private void UpdateView()
    {
        if (GameManager.Instance.GetSettingData().Music.Value)
        {
            On?.Invoke();
        }
        else
        {
            Off?.Invoke();
        }
    }
}

#if UNITY_EDITOR
namespace Lean.Gui.Editor
{
    using UnityEditor;
    using TARGET = ButtonMusic;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(TARGET))]
    public class ButtonMusic_Editor : ButtonGame_Editor
    {
        protected override void DrawSelectableEvents(bool showUnusedEvents)
        {
            TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

            base.DrawSelectableEvents(showUnusedEvents);

            if (showUnusedEvents == true || Any(tgts, t => t.OnDown.GetPersistentEventCount() > 0))
            {
                Draw("On", "");
            }

            if (showUnusedEvents == true || Any(tgts, t => t.OnClick.GetPersistentEventCount() > 0))
            {
                Draw("Off", "");
            }
        }
    }
}
#endif