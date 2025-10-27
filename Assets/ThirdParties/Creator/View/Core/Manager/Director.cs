using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
using YNL.Utilities.Extensions;
using UnityTimer;

namespace Creator
{
    public class Director : ManagerDirector
    {
        protected static string m_LoadingSceneName;
        protected static Controller m_LoadingController;

        public static string LoadingSceneName
        {
            set
            {
                m_LoadingSceneName = value;
                SceneManager.LoadScene(m_LoadingSceneName, LoadSceneMode.Additive);
            }
            get
            {
                return m_LoadingSceneName;
            }
        }

        public static bool HasLoading() => m_LoadingController != null;

        static Director()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;

            SceneAnimationDuration = 0.15f;

            Object = ((GameObject)GameObject.Instantiate(Resources.Load("ManagerObject"))).GetComponent<ManagerObject>();

            Object.gameObject.name = "ManagerObject";

            ApplyQuality();
        }

        static void ApplyQuality()
        {
            int qualityIndex = DetectQualityLevel();
            QualitySettings.SetQualityLevel(qualityIndex, true);

            // Cài FPS theo từng mức
            switch (qualityIndex)
            {
                case 0: // Low
                    Application.targetFrameRate = 30;
                    break;
                case 1: // Medium
                    Application.targetFrameRate = 60;
                    break;
                case 2: // High
                    Application.targetFrameRate = 60; // hoặc 120 nếu muốn cho device cao cấp
                    break;
            }

            QualitySettings.vSyncCount = 0; // tắt VSync để dùng targetFrameRate
            Console.Log($"👉 Applied Quality: {QualitySettings.names[qualityIndex]} | FPS: {Application.targetFrameRate}");
        }

        static int DetectQualityLevel()
        {
#if UNITY_EDITOR
            return 2; // Medium khi chạy Editor
#elif UNITY_ANDROID
        // Android phân loại theo RAM + GPU
        if (SystemInfo.systemMemorySize < 3000 || SystemInfo.graphicsMemorySize < 500)
            return 0; // Low
        else if (SystemInfo.systemMemorySize < 5000)
            return 1; // Medium
        else
            return 2; // High
#elif UNITY_IOS
        // iOS phân loại theo model
        string device = SystemInfo.deviceModel.ToLower();
        if (device.Contains("iphone6") || device.Contains("iphone7") || device.Contains("ipad5"))
            return 0; // Low
        else if (device.Contains("iphone8") || device.Contains("iphone9") || device.Contains("ipad6"))
            return 1; // Medium
        else
            return 2; // High
#else
        return 1; // Default Medium
#endif
        }

        #region Loading
        public static void LoadingAnimation(bool active)
        {
            if (m_LoadingController != null)
            {
                if (active)
                {
                    (m_LoadingController as ILoading).ShowLoading();
                }
                else
                {
                    (m_LoadingController as ILoading).HideLoading();
                }
            }
        }
        #endregion

        #region Controller
        public static void OnShown(Controller controller)
        {
            if (controller.FullScreen && m_ControllerStack.Count > 1)
            {
                ActivatePreviousController(controller, false);
            }

            Timer.Register(Director.SceneAnimationDuration, () =>
            {
                controller.OnShown();
                if (controller.Data != null && controller.Data.onShown != null)
                {
                    controller.Data.onShown();
                }
            }, autoDestroyOwner: controller);

            Object.ShieldOff();
        }

        public static void StartShow(Controller controller)
        {
            HideController(controller, false);
        }

        public static void OnHidden(Controller controller)
        {
            controller.OnHidden();
            if (controller.Data.onHidden != null)
            {
                controller.Data.onHidden();
            }

            Unload();

            if (m_ControllerStack.Count > 0)
            {
                var currentController = m_ControllerStack.Peek();
                currentController.OnReFocus();
            }

            Object.ShieldOff();
        }

        public static void OnFadedIn()
        {
            m_MainController.OnShown();
        }

        public static void OnFadedOut()
        {
            if (m_MainController != null)
            {
                m_MainController.OnHidden();
            }
            LoadrAsync(m_MainSceneName);
        }

        static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // Get Controller
            var controller = GetController(scene);

            if (controller == null)
                return;

            if (controller.SceneName() == LoadingSceneName)
            {
                SettingController(ref m_LoadingController, 90);
                return;
            }

            void SettingController(ref Controller controllerRef, int sortingOrder)
            {
                controllerRef = controller;
                controllerRef.HasShield = false;
                controllerRef.FullScreen = false;
                controllerRef.UseCameraUI = true;
                controller.SetupCanvas(sortingOrder);
                controllerRef.gameObject.SetActive(false);
                GameObject.DontDestroyOnLoad(controllerRef.gameObject);
            }

            // Single Mode automatically destroy all scenes, so we have to clear the stack.
            if (mode == LoadSceneMode.Single)
            {
                m_ControllerStack.Clear();
            }

            // Unload resources and collect GC.
            Resources.UnloadUnusedAssets();
            // System.GC.Collect();

            // Get Data
            if (m_DataQueue.Count == 0)
            {
                m_DataQueue.Enqueue(new Data(null, scene.name, null, null));
            }

            Data data = m_DataQueue.Dequeue();
            while (data.sceneName != scene.name && m_DataQueue.Count > 0)
            {
                data = m_DataQueue.Dequeue();
            }

            if (data == null)
            {
                data = new Data(null, scene.name, null, null);
            }

            data.scene = scene;

            // Push the current scene to the stack.
            m_ControllerStack.Push(controller);

            // Setup controller
            controller.Data = data;
            controller.HasShield = data.hasShield;
            controller.SetupCanvas(m_ControllerStack.Count - 1);
            controller.OnActive(data.data);
            controller.CreateShield();
            controller.EventShow();
            // Animation
            if (m_ControllerStack.Count == 1)
            {
                MCamera.SetupBaseAndOverlayCameras(controller.Camera, Object.UICamera);

                // Main Scene
                m_MainController = controller;
                if (string.IsNullOrEmpty(m_MainSceneName))
                {
                    m_MainSceneName = scene.name;
                }

                // Fade
                Object.FadeInScene();
            }
            else
            {
                // Popup Scene
                controller.Show();
            }
        }
        #endregion

        #region LoadingScene

        static void LoadrAsync(string sceneName)
        {
            if (HasLoading())
            {
                Director.LoadingAnimation(true);
                m_LoadingController.StopAllCoroutines();
                m_LoadingController.StartCoroutine(LoadYourAsyncScene(sceneName));
            }
            else
            {
                SceneManager.LoadScene(m_MainSceneName, LoadSceneMode.Single);
            }
        }

        static IEnumerator LoadYourAsyncScene(string sceneName)
        {
            yield return new WaitForSeconds(0.5f);

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);

            while (!asyncLoad.isDone)
            {
                yield return new WaitForEndOfFrame();
            }
        }
        #endregion
    }
}
