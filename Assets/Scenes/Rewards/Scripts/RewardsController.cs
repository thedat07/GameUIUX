using Creator;
using Lean.Touch;
using UnityTimer;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using BrunoMikoski.AnimationSequencer;
using DG.Tweening;

public class RewardsController : Controller
{
    public const string REWARDS_SCENE_NAME = "PopupRewards";

    public override string SceneName()
    {
        return REWARDS_SCENE_NAME;
    }

    [SerializeField] DataMethod m_Data;


    [Header("View")]

    public RewardsItemView rewardsItemView;

    public Transform pointSpawn;

    public TextMeshProUGUI txtTile;

    private List<RewardsItemView> m_Spawns;

    public override void OnActive(object data)
    {
        if (data != null)
        {
            m_Data = data as DataMethod;
            Init();
        }
        GameManager.Instance.GetSettingModelView().PlaySound(TypeAudio.IAP);
    }

    public override void OnShown()
    {
        SpawnRewards();
    }

    void Init()
    {
        txtTile.text = m_Data.title;
    }
    
    private void SpawnRewards()
    {
        m_Spawns = new List<RewardsItemView>();
        float delay = 0;
        foreach (var item in m_Data.data)
        {
            var view = Instantiate(rewardsItemView, pointSpawn);
            view.Init(item);
            if (view.transform.GetChild(1).TryGetComponent<AnimationSequencerController>(out AnimationSequencerController anim))
            {
                m_Spawns.Add(view);
                Timer.Register(delay, anim.Play, autoDestroyOwner: this);
                delay += 0.1f;
            }
        }
    }

    private void EffectHide()
    {
        if (!GameManager.Instance.GetMasterModelView().IsPlay)
        {
            this.Animation.shield.gameObject.SetActive(false);
            RewardsItemView coin = m_Spawns.Find(x => x.info.GetDataType() == MasterDataType.Money);
            RewardsItemView heart = m_Spawns.Find(x => x.info.GetDataType() == MasterDataType.Lives || x.info.GetDataType() == MasterDataType.LivesInfinity);

            Sequence mySequence = DOTween.Sequence();
            mySequence.Join(Move(coin, HomeController.Instance.pointCoin));
            mySequence.Join(Move(heart, HomeController.Instance.pointHeart));

            m_Spawns.RemoveAll(x =>
                    x.info.GetDataType() == MasterDataType.Money ||
                    x.info.GetDataType() == MasterDataType.Lives ||
                    x.info.GetDataType() == MasterDataType.LivesInfinity);

            foreach (var item in m_Spawns)
            {
                mySequence.Join(Move(item, HomeController.Instance.pointPlay));
            }

            mySequence.OnKill(OnKeyBack);
        }
        else
        {
            OnKeyBack();
        }

        Sequence Move(RewardsItemView view, Transform pointMove)
        {
            if (view != null)
            {
                return DOTween.Sequence()
                    .Append(view.transform.DOMove(pointMove.position, 0.75f).SetEase(Ease.InBack))
                    .Join(view.transform.DOScale(Vector3.one * 0.5f, 0.75f).SetEase(Ease.Linear))
                    .SetDelay(0.5f)
                    .SetLink(gameObject, LinkBehaviour.KillOnDestroy)
                    .OnComplete(() =>
                    {
                        view.gameObject.SetActive(false);
                    });

            }
            return null;
        }
    }
}