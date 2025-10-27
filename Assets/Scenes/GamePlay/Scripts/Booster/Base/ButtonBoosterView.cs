using UnityEngine.Events;
using UniRx;
using UnityEngine;
using DG.Tweening;

public partial class ButtonBoosterManager
{
    void ViewInit()
    {
        m_ArrowAnim = GetComponent<Animation>();
        CheckLock();
        ViewSub();
        GetBoosterData().Value.Subscribe((x) => { ViewSub(); }).AddTo(this);
        GamePlayController.Instance.activeArrow.Subscribe(ViewArrow).AddTo(this);
    }

    protected virtual void ViewArrow(bool active)
    {
        if (GetBoosterData().IsLock() || GetBoosterData().Get() <= 0)
        {
            m_Arrow.SetActive(false);
        }
        else
        {
            m_Arrow.SetActive(active);
            if (active)
            {
                m_ArrowAnim.transform.localScale = Vector3.one;
                viewObject.transform.localScale = Vector3.one;
                m_ArrowAnim.transform.DOScale(1, 0.25f).SetEase(Ease.OutBack).SetLink(gameObject, LinkBehaviour.KillOnDestroy).OnComplete(() =>
                {
                    m_ArrowAnim.Play("IdleArrow");
                });
            }
            else
            {
                m_ArrowAnim.Stop();
            }
        }
    }

    void ViewInfo()
    {
        bool activeNoti = GetBoosterData().Get() > 0 ? true : false;
        txtInfo.text = string.Format("{0}", GetBoosterData().Get());
        noti.SetActive(activeNoti);
        plus.SetActive(!activeNoti);
    }

    void ViewLock()
    {
        bool isLock = GetBoosterData().IsLock();
        txtLevelLock.text = string.Format("Level {0}", GetBoosterData().GetLevelUnlock());
        viewObject.SetActive(!isLock);
        lockObject.SetActive(isLock);
    }

    void CheckLock()
    {
        if (GameManager.Instance.GetMasterData().UnLock(type))
        {
            UnLockNewBoosterData data = new UnLockNewBoosterData(type, GetComponent<RectTransform>(), iconBG);
            Creator.ManagerDirector.PushScene(UnLockNewBoosterController.UNLOCKNEWBOOSTER_SCENE_NAME, data, () =>
            {
                GetButtonGame().interactable = true;
            }, () =>
            {
                GetButtonGame().OnClick.Invoke();
            });
        }
    }

    void ViewSub()
    {
        ViewInfo();
        ViewLock();
    }
}