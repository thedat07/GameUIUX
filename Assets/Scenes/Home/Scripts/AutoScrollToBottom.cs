using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AutoScrollToBottom : MonoBehaviour, IEndDragHandler
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private float snapThreshold = 0.1f;  // khoảng cách để auto snap
    [SerializeField] private float snapSpeed = 5f;       // tốc độ trượt xuống

    private bool snapping = false;


    void Start()
    {
        scrollRect.verticalNormalizedPosition = 0.1f;
        snapping = true;
    }

    void Update()
    {
        if (snapping)
        {
            scrollRect.verticalNormalizedPosition = Mathf.Lerp(
                scrollRect.verticalNormalizedPosition,
                0f,                           // target = đáy
                Time.deltaTime * snapSpeed
            );

            // nếu gần 0 thì fix cứng
            if (scrollRect.verticalNormalizedPosition <= 0.001f)
            {
                scrollRect.verticalNormalizedPosition = 0f;
                snapping = false;
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Nếu gần đáy thì auto snap
        if (scrollRect.verticalNormalizedPosition <= snapThreshold)
        {
            snapping = true;
        }
    }
}
