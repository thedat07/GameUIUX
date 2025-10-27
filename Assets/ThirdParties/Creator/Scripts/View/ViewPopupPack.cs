using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DanielLochner.Assets.SimpleScrollSnap;

public class ViewPopupPack : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private SimpleScrollSnap scrollSnap;
    [SerializeField] private float swapInterval = 3f; // thời gian delay giữa các swap

    private int currentIndex = 0;
    private float timer;

    void OnEnable()
    {
        // Lắng nghe event khi panel được chọn (vuốt đổi)
        scrollSnap.OnPanelSelected.AddListener(OnPanelChanged);
    }

    void OnDisable()
    {
        scrollSnap.OnPanelSelected.RemoveListener(OnPanelChanged);
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= swapInterval)
        {
            timer = 0f;
            Swap();
        }
    }

    void Swap()
    {
        // Có 2 pack -> index 0 và 1
        currentIndex = (currentIndex == 0) ? 1 : 0;
        scrollSnap.GoToPanel(currentIndex);
    }

    // Reset khi người chơi click vào (dù chưa vuốt)
    public void OnPointerDown(PointerEventData eventData)
    {
        ResetTimer();
    }

    // Reset khi panel thay đổi (người chơi vuốt)
    private void OnPanelChanged(int index)
    {
        ResetTimer();
        currentIndex = index;
    }

    private void ResetTimer()
    {
        timer = 0f;
    }
}
