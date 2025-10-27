using UnityEngine;
using UnityEngine.UI;
using YNL.Utilities.Extensions;

public class PointMask : MonoBehaviour
{
    public Transform point;

    public Image image;

    [SerializeField] RectTransform m_RectTransform;

    RectTransform m_RectTransformMasking;

    PopupMaskController m_Popup;

    public RectTransform GetRect()
    {
        if (m_RectTransform == null)
        {
            m_RectTransform = GetComponent<RectTransform>();
        }
        return m_RectTransform;
    }

    public RectTransform GetRectMasking()
    {
        if (m_RectTransformMasking == null)
        {
            m_RectTransformMasking = transform.GetChild(0).GetComponent<RectTransform>();
        }
        return m_RectTransformMasking;
    }

    public void Init(Transform point, PopupMaskController popup, Sprite sprite, float scale)
    {
        this.point = point;
        this.m_Popup = popup;

        SettingImage();

        UpdateView();

        void SettingImage()
        {
            image.sprite = sprite;
            image.ReSize();
            Vector3 scaleTo = Vector3.one * scale;
            image.transform.localScale = scaleTo;
            image.transform.rotation = point.rotation;
        }
    }

    void UpdateView()
    {
        if (point != null)
        {
            GetRect().WorldToScreenSpace(point.transform.position, m_Popup.Canvas.worldCamera, m_Popup.contentPoint, m_Popup.GetSize());
        }
    }
}
