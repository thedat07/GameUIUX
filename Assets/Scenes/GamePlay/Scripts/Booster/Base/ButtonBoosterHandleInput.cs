public partial class ButtonBoosterManager
{
    void HandleInputInit()
    {
        GetButtonGame().OnClick.AddListener(OnClick);
    }

    void OnClick()
    {
        if (!GetBoosterData().IsLock())
        {
            if (GetBoosterData().Get() <= 0)
            {
                GameManager.Instance.GetSettingModelView().PlaySound(TypeAudio.BoosterClear);
                Creator.ManagerDirector.PushScene(BuyBoosterController.BUYBOOSTER_SCENE_NAME, new BuyBoosterData(type));
            }
            else
            {

                if (CanUseBooster())
                {
                    UseBooster();
                    GameManager.Instance.GetSettingModelView().PlaySound(HelperCreator.GetBoosterAdioType(type));
                }
                else
                {
                    GameManager.Instance.GetSettingModelView().PlaySound(TypeAudio.ButtonClick);
                }
            }
        }
        else
        {
            GameManager.Instance.GetSettingModelView().PlaySound(TypeAudio.ButtonClick);
        }
    }
}