using Coffee.UIEffects;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class ButtonHomeFeature : MonoBehaviour
{
    protected abstract TypeFeature GetTypeFeature();

    protected abstract FeatureData GetData();

    private FeatureModel m_Model => GameManager.Instance.GetFeatureData();

    [SerializeField] ButtonGame m_Btn;

    [Header("Info Icon")]
    [SerializeField] GameObject iconHideUnLock;
    [SerializeField] UIEffect[] m_Grays;

    protected virtual string GetLevelLock() => string.Format("Level {0}", StaticDataFeature.FeatureUnlockLevels[GetTypeFeature()]);

    public virtual GameObject GetContent() => gameObject;
    void Awake()
    {
        if (StaticDataFeature.FeatureActive.TryGetValue(GetTypeFeature(), out bool isActive) && !isActive)
        {
            ActiveObject(false);
            return;
        }
        else
        {
            if (StaticDataFeature.FeatureUnlockLevels.TryGetValue(GetTypeFeature(), out int levelUnlock))
            {
                int currentLevel = GameManager.Instance.GetMasterData().dataStage.Get();
                bool canShow = currentLevel >= levelUnlock || (levelUnlock - currentLevel) <= 10;
                ActiveObject(canShow);
            }
        }

        if (GetData() == null)
        {
            m_Btn.OnClick.AddListener(() =>
           {
               Creator.Director.Object.ShowInfo(string.Format("Feature unlocked at level {0}", StaticDataFeature.FeatureUnlockLevels[GetTypeFeature()]));
           });
            ViewLock();
            SettingGray(0.85f);
        }
        else
        {
            m_Btn.OnClick.AddListener(OnClick);
            View();
        }
    }

    protected virtual void ActiveObject(bool active)
    {
        GetContent().SetActive(active);
    }

    private void View()
    {
        if (IsLock())
        {
            ViewLock();
            SettingGray(0.85f);
        }
        else
        {
            ViewUnlockLock();
            SettingGray(0f);
        }
    }

    protected abstract void ViewLock();

    protected abstract void ViewUnlockLock();

    public bool IsLock()
    {
        if (StaticDataFeature.FeatureUnlockLevels.ContainsKey(GetTypeFeature()))
        {
            int levelUnlock = StaticDataFeature.FeatureUnlockLevels[GetTypeFeature()];

            if (m_Model.IsUnlock(levelUnlock))
            {
                return false;
            }
        }

        return true;
    }

    private void OnClick()
    {
        if (FirebaseEventLogger.GetCategory() == FirebaseEventLogger.Category.Home)
        {
            FirebaseEventLogger.LogButtonClick(GetTypeFeature().ToString().ToLower(), "Khi user click feature ở màn hình Home");
        }
        else
        {
            FirebaseEventLogger.LogButtonClick(GetTypeFeature().ToString().ToLower(), "Khi user click feature ở màn hình InGame");
        }

        if (GetData() == null) return;
        if (!IsLock())
        {
            Click();
        }
        else
        {
            Creator.Director.Object.ShowInfo(string.Format("Feature unlocked at level {0}", StaticDataFeature.FeatureUnlockLevels[GetTypeFeature()]));
        }
    }

    protected virtual void Click()
    {

    }

    protected virtual void SettingGray(float amount)
    {
        foreach (var item in m_Grays)
        {
            item.toneIntensity = amount;
        }
        if (iconHideUnLock)
            iconHideUnLock.SetActive(amount == 0 ? false : true);
    }
}
