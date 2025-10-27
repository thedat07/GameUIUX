using Creator;
using UnityEngine;

public class ShopMiniController : Controller
{
    public const string SHOPMINI_SCENE_NAME = "ShopMini";

    public override string SceneName()
    {
        return SHOPMINI_SCENE_NAME;
    }

    public Transform pointCoin;

    public Transform pointHome;

    public Transform pointGamePlay;

    void Start()
    {
        pointCoin.position = GameManager.Instance.GetMasterModelView().IsPlay ? pointGamePlay.position : pointHome.position;
    }
}