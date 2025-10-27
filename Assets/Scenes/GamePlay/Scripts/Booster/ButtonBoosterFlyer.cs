using Core.Events;
using UnityEngine;
using UniRx;

public partial class ButtonBoosterFlyer : ButtonBoosterManager
{
    public GameObject View;

    [SerializeField] CanvasGroup m_lstButton;

    protected override void Init()
    {
        base.Init();
        GameEventSub.OnFreezeTimeChange.Subscribe(_ => { IsUse(); }).AddTo(this);
    }

    public override void IsUse()
    {
        base.IsUse();
        OnClose();
    }

    protected override void UseBooster2()
    {
        base.UseBooster2();
        View.SetActive(true);
        m_lstButton.interactable = false;
    }

    public override void OnClose()
    {
        base.OnClose();
        View.SetActive(false);
        m_lstButton.interactable = true;
        BoosterManager.Instance.CancelBooster(BoosterManager.BoosterType.Booster2);
    }
}
