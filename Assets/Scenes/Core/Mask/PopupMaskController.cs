
using UnityEngine;
using Creator;
using UnityEngine.UI;
using Lean.Pool;
using System.Collections.Generic;
using TMPro;

public interface IMask
{
    void ShowMask(PopupMaskData data);
    void ShowMask(List<PopupMaskData> datas);
    bool UpdateMaskStep();
    void HideMask();
}

public class PopupMaskController : Controller, IMask
{
    public const string SCENE_NAME = "PopupMask";

    public override string SceneName()
    {
        return SCENE_NAME;
    }

    [Header("Mask")]
    public CanvasGroup canvasMask;
    public LeanGameObjectPool point;
    public RectTransform contentPoint;

    [Header("Text")]
    public GameObject txtPanel;
    public TextMeshProUGUI txtInfo;

    [Header("Hand")]
    public GameObject hand;
    public Image mask;

    int m_Step;
    Camera m_Cam;
    List<PointMask> m_PointMasks;
    List<PopupMaskData> m_PopupMaskDatas;

    public float GetSize()
    {
        if (m_Cam)
            return Canvas.worldCamera.orthographicSize / m_Cam.orthographicSize;
        else return 0;
    }

    private void SetCam(SettingCamTutor settingCamTutor)
    {
        if (settingCamTutor.baseCam != null)
        {
            m_Cam = settingCamTutor.baseCam;
            Canvas.worldCamera.transform.position = m_Cam.transform.position;
        }
        else
        {
            m_Cam = Creator.Director.Object.UICamera;
            Canvas.worldCamera.transform.position = Creator.Director.Object.UICamera.transform.position;
        }
    }

    public void ShowMask(PopupMaskData data)
    {
        ClearMask();
        OnShow();
        View(data);
    }

    public void ShowMask(List<PopupMaskData> datas)
    {
        ClearMask();
        OnShow();
        m_PopupMaskDatas.AddRange(datas);
        m_Step = 0;
        View(m_PopupMaskDatas[m_Step]);
    }

    public void HideMask()
    {
        canvasMask.alpha = 0;
        gameObject.SetActive(false);
        ClearMask();
    }

    public bool UpdateMaskStep()
    {
        point.DespawnAll();
        m_PointMasks = new List<PointMask>();
        m_Step++;
        if (m_Step < m_PopupMaskDatas.Count)
        {
            View(m_PopupMaskDatas[m_Step]);
            return true;
        }
        return false;
    }

    private void View(PopupMaskData data)
    {
        data.callback?.Invoke();

        mask.color = new Color(0, 0, 0, data.mask);

        SetCam(data.settingCam);

        if (data.point != null)
            txtPanel.transform.position = data.point.transform.position;
        else
            txtPanel.transform.position = transform.position;

        UpdateText(data.info);
        foreach (var item in data.lstInfo)
        {
            SetMaskUI(item.Item1, item.Item2, item.Item3);
        }
        ShowHand(data.showHand, data.flipHand);
    }

    void UpdateText(string text = "")
    {
        if (text == "")
        {
            txtPanel.gameObject.SetActive(false);
        }
        else
        {
            txtPanel.gameObject.SetActive(true);
        }
        txtInfo.text = text;
    }

    void ClearMask()
    {
        point.DespawnAll();
        m_PointMasks = new List<PointMask>();
        m_PopupMaskDatas = new List<PopupMaskData>();
    }

    void ShowHand(bool show, bool flipHand)
    {
        hand.SetActive(show);
        if (show == true)
        {
            if (m_PointMasks != null && m_PointMasks.Count > 0)
            {
                hand.transform.position = m_PointMasks[0].transform.position;
                hand.SetActive(true);
            }
            else
            {
                hand.SetActive(false);
            }
        }
    }

    void OnShow()
    {
        gameObject.SetActive(true);
        canvasMask.alpha = 1;
    }

    public void SetMaskUI(Sprite sprite, Transform pointSpawn, float scale)
    {
        var pointMask = point.Spawn(contentPoint).GetComponent<PointMask>();
        pointMask.Init(pointSpawn, this, sprite, scale);
        pointMask.transform.localScale = Vector3.one;
        m_PointMasks.Add(pointMask);
    }

    public bool IsPointAllowed(Vector2 screenPoint, Camera camera)
    {
        if (
            screenPoint.x < 0 || screenPoint.x > Screen.width ||
            screenPoint.y < 0 || screenPoint.y > Screen.height)
        {
            return false;
        }

        foreach (var area in m_PointMasks)
        {
            if (area != null)
            {
                bool isInside = RectTransformUtility.RectangleContainsScreenPoint(
                                area.GetRectMasking(),
                                screenPoint,
                                camera
                            );

                if (isInside)
                {
                    return true;
                }
            }
        }

        return false;
    }
}