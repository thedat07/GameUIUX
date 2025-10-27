using UnityUtilities;
using UniRx;
using System.Collections.Generic;

[System.Serializable]
public class ProfileData : ListIntMasterData
{
    private ReactiveProperty<bool> _noti;

    public IReadOnlyReactiveProperty<bool> Noti => _noti;

    private ReactiveProperty<List<int>> _valueID;

    public IReadOnlyReactiveProperty<List<int>> ValueID => _valueID;

    public ProfileData(List<int> defaultValue, MasterDataType type) : base(defaultValue, type)
    {
        bool noti = StaticData.NotiProfile;

        _valueID = new ReactiveProperty<List<int>>(Get());

        _noti = new ReactiveProperty<bool>(noti);
    }

    public bool IsActive(int id) => _valueID.Value.Contains(id);

    public void HideNoti()
    {
        if (StaticData.NotiProfile)
        {
            StaticData.NotiProfile = false;
            _noti.Value = StaticData.NotiProfile;
        }
    }

    public override void Post(int id)
    {
        var lst = _valueID.Value;
        if (!IsActive(id))
        {
            base.Post(id);
        }
    }
}
