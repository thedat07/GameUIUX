using UnityUtilities;
using UniRx;

[System.Serializable]
public class FeatureLukySpin : FeatureData, IFeatureUpdatable
{
    public class Data
    {
        private const string KEY_FREE_SPIN_USED = "freeSpinUsed";
        private const string KEY_ADS_SPINS_USED = "adsSpinsUsed";
        private const string KEY_LAST_SPIN_DATE = "lastSpinDate";
        private const string KEY_SPIN_COUNT = "spinCount";

        public BoolReactiveProperty freeSpinUsed;
        public IntReactiveProperty adsSpinsUsed;
        public StringReactiveProperty lastSpinDate;

        public IntReactiveProperty spinCount;

        public Data()
        {
            freeSpinUsed = new BoolReactiveProperty(false);
            adsSpinsUsed = new IntReactiveProperty(0);
            lastSpinDate = new StringReactiveProperty(NetworkTime.UTC.Date.ToString());
            spinCount = new IntReactiveProperty(0);
        }

        public void Reset()
        {
            freeSpinUsed.Value = false;
            adsSpinsUsed.Value = 0;
            lastSpinDate.Value = NetworkTime.UTC.Date.ToString();
            spinCount.Value = 0;
        }

        public void Save(TypeFeature type)
        {
            SaveExtensions.PutFeature(type, KEY_FREE_SPIN_USED, freeSpinUsed.Value);
            SaveExtensions.PutFeature(type, KEY_ADS_SPINS_USED, adsSpinsUsed.Value);
            SaveExtensions.PutFeature(type, KEY_LAST_SPIN_DATE, lastSpinDate.Value);
            SaveExtensions.PutFeature(type, KEY_SPIN_COUNT, spinCount.Value);
        }

        public void Load(TypeFeature type)
        {
            freeSpinUsed.Value = SaveExtensions.GetFeature(type, KEY_FREE_SPIN_USED, false);
            adsSpinsUsed.Value = SaveExtensions.GetFeature(type, KEY_ADS_SPINS_USED, 0);
            lastSpinDate.Value = SaveExtensions.GetFeature(type, KEY_LAST_SPIN_DATE, NetworkTime.UTC.Date.ToString());
            spinCount.Value = SaveExtensions.GetFeature(type, KEY_SPIN_COUNT, 0);
        }
    }

    private int m_MaxDailySpins = StaticData.MaxAdsSpins;

    private Data m_Data = new Data();

    public Data GetData() => m_Data;

    public int SpinCount() => m_Data.spinCount.Value;

    public FeatureLukySpin(TypeFeature type, int levelUnlock = 0) : base(type, levelUnlock)
    {
        LoadData();
        CheckDailyReset();
    }

    // Kiểm tra có được spin không
    public bool CanSpin(bool isAdsSpin)
    {
        if (!IsUnlock())
            return false;

        CheckDailyReset();

        if (!isAdsSpin)
        {
            // free spin
            return !m_Data.freeSpinUsed.Value;
        }
        else
        {
            // ads spin
            return m_Data.adsSpinsUsed.Value < m_MaxDailySpins;
        }
    }

    // Thực hiện spin
    public void DoSpin(bool isAdsSpin)
    {
        if (!isAdsSpin)
        {
            m_Data.freeSpinUsed.Value = true;
        }
        else
        {
            m_Data.adsSpinsUsed.Value++;
        }
        SaveData();
    }

    // Lấy số lượt còn lại
    public (int freeLeft, int adsLeft) GetRemainingSpins()
    {
        CheckDailyReset();

        int freeLeft = m_Data.freeSpinUsed.Value ? 0 : 1;
        int adsLeft = m_MaxDailySpins - m_Data.adsSpinsUsed.Value;

        return (freeLeft, adsLeft);
    }

    // Reset theo ngày user
    public void CheckDailyReset()
    {
        string today = NetworkTime.UTC.Date.ToString();

        if (m_Data.lastSpinDate.Value != today)
        {
            m_Data.Reset();
            SaveData();
        }
    }

    private void LoadData()
    {
        m_Data.Load(m_Type);
    }

    public void UpdateSpinCount(bool reset)
    {
        int spinCount = m_Data.spinCount.Value;
        if (reset)
        {
            spinCount = 0;
        }
        else
        {
            spinCount += 1;
        }
        m_Data.spinCount.Value = spinCount;
        SaveData();
    }

    private void SaveData()
    {
        m_Data.Save(m_Type);
    }

    public void CustomUpdate()
    {
        CheckDailyReset();
    }

    public string GetTime()
    {
        // Lấy ngày lưu lần cuối
        System.DateTime lastDate = System.DateTime.Parse(m_Data.lastSpinDate.Value).Date;

        // Mốc reset = ngày tiếp theo lúc 00:00 (UTC)
        System.DateTime nextReset = lastDate.AddDays(1);

        // Thời gian hiện tại (UTC)
        System.DateTime now = NetworkTime.UTC;

        // Nếu đã qua reset thì trả về 00:00:00
        if (now >= nextReset)
            return "00:00:00";

        // Thời gian còn lại
        System.TimeSpan remaining = nextReset - now;
        string formatD = "{0}d {1}h"; string formatH = "{0}h {1}m";
        string formatM = "{0}m {1}s"; string formatZero = "--";
        return UnityUtilities.CountdownTextUtilities.FormatCountdownLives(remaining, formatD, formatH, formatM, formatZero);
    }
}