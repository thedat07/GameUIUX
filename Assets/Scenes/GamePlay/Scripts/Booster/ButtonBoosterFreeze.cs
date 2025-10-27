using Core.Events;
using UnityEngine;
using UniRx;

public partial class ButtonBoosterFreeze : ButtonBoosterManager
{
    [SerializeField] bool m_Active;


    protected override void Init()
    {
        base.Init();
        GameEventSub.OnFreezeTimeChange.Subscribe(x => { ViewFreeze(x.a); }).AddTo(this);
    }

    void ViewFreeze(float time)
    {
        m_Active = (time > 0);
        GetCanvasGroup().interactable = !(time > 0);
    }

    protected override void ViewArrow(bool active)
    {
        if (!m_Active)
        {
            base.ViewArrow(active);
        }
        else
        {
            m_Arrow.SetActive(false);
        }
    }
}
