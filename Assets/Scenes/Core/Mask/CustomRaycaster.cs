using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomRaycaster : GraphicRaycaster
{
    public PopupMaskController raycastFilter;

    public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
    {
        if (raycastFilter == null || !raycastFilter.IsPointAllowed(eventData.position, eventCamera))
        {
            base.Raycast(eventData, resultAppendList);
        }
    }
}