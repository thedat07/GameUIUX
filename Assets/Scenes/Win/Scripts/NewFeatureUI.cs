using System.Collections;
using System.Collections.Generic;
using BrunoMikoski.AnimationSequencer;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewFeatureUI : MonoBehaviour
{
    public SoNewFeature soNewFeature;

    public Image iconAvartar;

    public Slider slider;

    public TextMeshProUGUI txtInfo;
    
    public TextMeshProUGUI txtName;

    public GameObject objectLock;

    public GameObject objectUnLock;

    public GameObject[] activeEffect;

    public AnimationSequencerController anim;

    void Start()
    {
        objectLock.SetActive(false);
        objectUnLock.SetActive(false);

        View();
    }

    public void View()
    {
        int level = GameManager.Instance.GetMasterData().dataStage.Get();

        SoNewFeature.Data target = null;
        SoNewFeature.Data prev = null;

        foreach (var d in soNewFeature.datas)
        {
            if (level <= d.requireLevel)
            {
                target = d;
                break;
            }
            prev = d;
        }

        if (target != null)
        {
            bool unlock = level == target.requireLevel;

            if (unlock)
            {
                StaticData.IsUnLocNewFeature = true;
            }

            iconAvartar.sprite = target.iconLock;

            int start = prev != null ? prev.requireLevel : 0;
            int end = target.requireLevel;

            int range = end - start;
            int currentValue = Mathf.Clamp(level - start, 0, range);

            slider.minValue = 0;
            slider.maxValue = range;

            slider.DOValue(currentValue, 1f).SetEase(Ease.OutCubic).SetLink(gameObject, LinkBehaviour.KillOnDestroy).OnComplete(() =>
            {
                if (unlock)
                {
                    objectUnLock.SetActive(false);
                    txtName.text = target.txtName;
                    GameManager.Instance.GetSettingModelView().PlaySound(TypeAudio.IAP);
                    UnityTimer.Timer.Register(0.25f, () =>
                    {
                        iconAvartar.sprite = target.iconUnLock;
                        iconAvartar.DOFade(0, 0f).SetLink(gameObject, LinkBehaviour.KillOnDestroy);
                        iconAvartar.DOFade(1f, 0.25f).SetLink(gameObject, LinkBehaviour.KillOnDestroy);

                    }, autoDestroyOwner: this);
                    anim.Play();
                }
            });

            txtInfo.text = $"{currentValue}/{range}";

            bool isUnlock = level >= end;
            objectLock.SetActive(!isUnlock);
            objectUnLock.SetActive(isUnlock);

            ActiveEffect(unlock);
        }
        else
        {
            iconAvartar.enabled = false;
            slider.value = slider.maxValue;
            txtInfo.text = "MAX";

            objectLock.SetActive(false);
            objectUnLock.SetActive(false);
        }
    }

    void ActiveEffect(bool active)
    {
        foreach (var item in activeEffect)
        {
            item.SetActive(active);
        }
    }
}
