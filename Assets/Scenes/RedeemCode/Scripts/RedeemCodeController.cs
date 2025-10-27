using Creator;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityUtilities;
using Sirenix.OdinInspector;
using TMPro;

public class RedeemCodeController : Controller
{
    public const string REDEEMCODE_SCENE_NAME = "RedeemCode";

    public override string SceneName()
    {
        return REDEEMCODE_SCENE_NAME;
    }

    [SerializeField] private TMP_InputField codeInput;

    private static readonly string key = "archer_game";

    [TextArea]
    public string original;

    [Button]
    public void Test()
    {
        if (original != null)
        {
            string encrypted = Encrypt(original);
            string decrypted = Decrypt(encrypted);
            UnityEngine.Debug.Log($"Original: {original}");
            UnityEngine.Debug.Log($"Encrypted: {encrypted}");
            UnityEngine.Debug.Log($"Decrypted: {decrypted}");
        }
    }

    [Button]
    public void OnSubmitCode()
    {
        string code = codeInput.text.Trim();

        string decrypted = Decrypt(code);

        if (!string.IsNullOrEmpty(decrypted))
        {
            GameManager.Instance.GetMasterData().SaveRaw(decrypted);
        }
        else
        {
            Creator.Director.Object.ShowInfo("Redeem Code Failed.");
        }
    }

    public string Encrypt(string plainText)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(key.PadRight(32).Substring(0, 32));
            aes.IV = new byte[16]; // IV mặc định = 0 (hoặc random tuỳ bạn)

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                using (var sw = new StreamWriter(cs))
                {
                    sw.Write(plainText);
                }

                // ms vẫn còn trong scope nên dùng được ở đây
                return Convert.ToBase64String(ms.ToArray());
            }
        }
    }

    public string Decrypt(string cipherText)
    {
        try
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key.PadRight(32).Substring(0, 32));
                aes.IV = new byte[16];

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (var ms = new MemoryStream(Convert.FromBase64String(cipherText)))
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (var sr = new StreamReader(cs))
                {
                    return sr.ReadToEnd();
                }
            }
        }
        catch (Exception ex)
        {
            return null;
        }
    }
}