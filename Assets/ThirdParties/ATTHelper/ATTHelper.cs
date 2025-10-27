#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.PackageManager.Requests;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEditor.SceneManagement;

class ATTHelper : MonoBehaviour
{
    static AddRequest Request;
    static RemoveRequest RemoveRequest;

    private const string packIdATT = "com.unity.ads.ios-support";

    [MenuItem("ATTHelper/Install")]
    static void Install()
    {
        Request = Client.Add(packIdATT);

        EditorApplication.update += Progress;
    }

    [MenuItem("ATTHelper/Uninstall")]
    static void Uninstall()
    {
        RemoveRequest = Client.Remove(packIdATT);

        EditorApplication.update += RemoveProgress;
    }

    static void CreateATTGameobject()
    {
        var listAtts = FindObjectsOfType<AttPermissionRequest>();

        if (listAtts.Length > 0) return;

        var att = new GameObject("ATTHelper");

        att.AddComponent<AttPermissionRequest>();

        EditorSceneManager.MarkAllScenesDirty();
    }

    static void DestroyATTGameobject()
    {
        var listAtts = FindObjectsOfType<AttPermissionRequest>();

        foreach (var r in listAtts)
        {
            DestroyImmediate(r.gameObject);
        }

        EditorSceneManager.MarkAllScenesDirty();
    }

    static void Progress()
    {
        if (Request.IsCompleted)
        {
            if (Request.Status == StatusCode.Success)
            {
                Debug.Log("Installed: " + Request.Result.packageId);

                CreateATTGameobject();
            }
            else if (Request.Status >= StatusCode.Failure)
                Debug.Log(Request.Error.message);

            EditorApplication.update -= Progress;
        }
    }

    static void RemoveProgress()
    {
        if (RemoveRequest.IsCompleted)
        {
            if (RemoveRequest.Status == StatusCode.Success)
            {
                DestroyATTGameobject();

                Debug.Log("Uninstalled: " + RemoveRequest.PackageIdOrName);
            }
            else if (RemoveRequest.Status >= StatusCode.Failure)
                Debug.Log(RemoveRequest.Error.message);

            EditorApplication.update -= Progress;
        }
    }

}
#endif