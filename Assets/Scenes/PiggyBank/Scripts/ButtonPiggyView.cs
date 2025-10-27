using UnityEngine;

public class ButtonPiggyView : ButtonHomePiggy
{
    protected override void Click()
    {
        Creator.ManagerDirector.PushScene(PiggyBankController.PIGGYBANK_SCENE_NAME);
    }
}
