using UnityEngine;

public class ButtonGameRemoveAds : ButtonGameIAP
{
    public GameObject[] objectActive;

    public override void UpdateView()
    {
        bool isActive = Gley.EasyIAP.API.IsActive(yourPorduct);

        this.interactable = isActive;

        foreach (var item in objectActive)
        {
            item.SetActive(isActive);
        }
    }
}

#if UNITY_EDITOR
namespace Lean.Gui.Editor
{
    using UnityEditor;
    using TARGET = ButtonGameRemoveAds;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(TARGET))]
    public class ButtonGameRemoveAds_Editor : ButtonGameIAP_Editor
    {
        protected override void DrawSelectableSettings()
        {
            base.DrawSelectableSettings();

            Draw("objectActive", "Object Hide When Remove Ads");
        }
    }
}
#endif