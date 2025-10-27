using UnityEngine;
using UnityEngine.Events;

public class StarRateView : MonoBehaviour
{
    public int index;

    public GameObject on;

    public GameObject off;

    public UnityEvent<int> eventClick;

    public void On()
    {
        on.SetActive(true);
        off.SetActive(false);
    }

    public void Off()
    {
        on.SetActive(false);
        off.SetActive(true);
    }

    public void OnClick()
    {
        eventClick?.Invoke(index);
    }
}
