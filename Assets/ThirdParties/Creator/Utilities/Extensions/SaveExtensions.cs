using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityUtilities
{
    public class SaveExtensions
    {
        public const string keyFileMasterData = "FileMasterData";
        public const string keyFileMasterData2 = "FileMasterData2";

        public const string keyFileData = "FileData";
        public const string keyFileSettingData = "FileSettingData";
        public const string keyFileAdsData = "FileAdsData";
        public const string keyFileQuestData = "FileFeatureData";
        public const string keyFileShopData = "FileShopData";
        public const string keyFileHomeData = "FileHomeData";

        public static bool KeyExists(TypeFeature type, string key)
        {
            string filePath = "FileGame";
            return ES3.KeyExists($"{type}_{key}", filePath);
        }

        // ========== Generic ==========

        /// <summary>GET: Lấy dữ liệu từ file</summary>
        public static T Get<T>(string key, T defaultValue, string filePath = "FileGame")
        {
            if (!ES3.KeyExists(key, filePath))
                Post(key, defaultValue, filePath); // POST nếu chưa tồn tại

            return ES3.Load(key, filePath, defaultValue);
        }

        /// <summary>POST: Tạo dữ liệu nếu chưa tồn tại</summary>
        public static void Post<T>(string key, T value, string filePath = "FileGame")
        {
            if (!ES3.KeyExists(key, filePath))
                ES3.Save(key, value, filePath);
        }

        /// <summary>PUT: Ghi đè dữ liệu</summary>
        public static void Put<T>(string key, T value, string filePath = "FileGame")
        {
            ES3.Save(key, value, filePath);
        }

        /// <summary>DELETE: Xóa dữ liệu</summary>
        public static void Delete(string key, string filePath = "FileGame")
        {
            if (ES3.KeyExists(key, filePath))
                ES3.DeleteKey(key, filePath);
        }

        // ========== MASTER DATA ==========

        public static T GetMaster<T>(MasterDataType type, string key, T defaultValue, bool dataGameServices = true)
        {
            if (dataGameServices)
                return Get<T>($"{type}_{key}", defaultValue, keyFileMasterData);
            else
                return Get<T>($"{type}_{key}", defaultValue, keyFileMasterData2);
        }

        public static void PutMaster<T>(MasterDataType type, string key, T value, bool dataGameServices = true)
        {
            if (dataGameServices)
                Put<T>($"{type}_{key}", value, keyFileMasterData);
            else
                Put<T>($"{type}_{key}", value, keyFileMasterData2);
        }

        public static void DeleteMaster(MasterDataType type, string key, bool dataGameServices = true)
        {
            if (dataGameServices)
                Delete($"{type}_{key}", keyFileMasterData);
            else
                Delete($"{type}_{key}", keyFileMasterData2);
        }

        // ========== SETTING DATA ==========

        public static T GetSetting<T>(string key, T defaultValue)
        {
            return Get<T>(key, defaultValue, keyFileSettingData);
        }

        public static void PutSetting<T>(string key, T value)
        {
            Put<T>(key, value, keyFileSettingData);
        }

        public static void DeleteSetting(string key)
        {
            Delete(key, keyFileSettingData);
        }

        // ========== ADS DATA ==========

        public static T GetAds<T>(string key, T defaultValue)
        {
            return Get<T>(key, defaultValue, keyFileAdsData);
        }

        public static void PutAds<T>(string key, T value)
        {
            Put<T>(key, value, keyFileAdsData);
        }

        public static void DeleteAds(string key)
        {
            Delete(key, keyFileAdsData);
        }

        // ========== Feature DATA ==========

        public static T GetFeature<T>(TypeFeature type, string key, T defaultValue)
        {
            return Get<T>($"{type}_{key}", defaultValue, keyFileQuestData);
        }

        public static void PutFeature<T>(TypeFeature type, string key, T value)
        {
            Put<T>($"{type}_{key}", value, keyFileQuestData);
        }

        public static void DeleteFeature(TypeFeature type, string key)
        {
            Delete($"{type}_{key}", keyFileQuestData);
        }

        public static ListES3<T> GetFeatureList<T>(TypeFeature type, string key, int levelUnlock = 0)
        {
            var info = ($"{type}_{key}", keyFileQuestData);
            return new ListES3<T>(info, levelUnlock);
        }

        // ========== SHOP DATA ==========

        public static T GetShop<T>(string key, T defaultValue)
        {
            return Get<T>(key, defaultValue, keyFileShopData);
        }

        public static void PutShop<T>(string key, T value)
        {
            Put<T>(key, value, keyFileShopData);
        }

        public static void DeleteShop(string key)
        {
            Delete(key, keyFileShopData);
        }

        // ========== HOME DATA ==========

        public static T GetHome<T>(string key, T defaultValue)
        {
            return Get<T>(key, defaultValue, keyFileHomeData);
        }

        public static void PutHome<T>(string key, T value)
        {
            Put<T>(key, value, keyFileHomeData);
        }

        public static void DeleteHome(string key)
        {
            Delete(key, keyFileHomeData);
        }
    }
}

public class ListES3<T> : List<T>
{
    private string key;
    private string filePath;

    private int m_LevelUnlock = 0;

    private bool UnLock()
    {
        return true;
    }

    public ListES3((string, string) info, int levelUnlock)
    {
        this.m_LevelUnlock = levelUnlock;
        this.key = info.Item1;
        this.filePath = info.Item2;

        if (ES3.KeyExists(key, filePath))
        {
            var loaded = ES3.Load<List<T>>(key, filePath);
            this.AddRange(loaded);
        }
    }

    public void AddES3(T item)
    {
        if (UnLock())
        {
            this.Add(item);
            ES3.Save(key, (this as List<T>), filePath);
        }
    }

    public void RemoveES3(T item)
    {
        if (this.Remove(item))
        {
            ES3.Save(key, (this as List<T>), filePath);
        }
    }

    public void ClearES3()
    {
        this.Clear();
        ES3.Save(key, (this as List<T>), filePath);
    }
}