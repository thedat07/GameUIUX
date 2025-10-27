
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class PopupMaskData
{
    public string info;
    public List<(Sprite, Transform, float)> lstInfo;
    public bool showHand;
    public Transform point;
    public bool flipHand;
    public SettingCamTutor settingCam;
    public UnityAction callback;
    public float mask = 0.78f;

    public PopupMaskData(
        string info, List<(Sprite, Transform, float)> lstInfo,
        SettingCamTutor settingCam, UnityAction callback = null,
        bool showHand = false,
        Transform point = null,
        bool flipHand = false,
        float mask = 0.78f)
    {
        this.info = info;
        this.lstInfo = lstInfo;
        this.showHand = showHand;
        this.point = point;
        this.flipHand = flipHand;
        this.settingCam = settingCam;
        this.callback = callback;
        this.mask = mask;
    }
}

public class SettingCamTutor
{
    public Camera baseCam;
    public int orderLayer;

    public SettingCamTutor(Camera baseCam, int orderLayer)
    {
        this.baseCam = baseCam;
        this.orderLayer = orderLayer;
    }
}