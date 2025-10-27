using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SecretButton : MonoBehaviour
{
    [SerializeField] private Button hiddenButton;
    [SerializeField] private float timeWindow = 2f;   // Thời gian giới hạn
    [SerializeField] private int requiredClicks = 10; // Số lần cần bấm

    private int clickCount;
    private float timer;
    private bool isCounting;

    void Start()
    {
        hiddenButton.onClick.AddListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
#if UNITY_EDITOR
        Creator.ManagerDirector.PushScene(PopupToolController.POPUPTOOL_SCENE_NAME);
#else
        if (GameManager.Instance.GetMasterModelView().IsTest)
        {
            Creator.ManagerDirector.PushScene(PopupToolController.POPUPTOOL_SCENE_NAME);
        }
        else
        {
            if (!isCounting)
            {
                isCounting = true;
                timer = timeWindow;
                clickCount = 0;
            }

            clickCount++;

            if (clickCount >= requiredClicks)
            {
                Creator.ManagerDirector.PushScene(PopupToolController.POPUPTOOL_SCENE_NAME);
                // TODO: gọi hàm active ở đây
                ResetCounter();
            }
        }
#endif
    }

    void Update()
    {
        if (isCounting && !GameManager.Instance.GetMasterModelView().IsTest)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                ResetCounter();
            }
        }
    }

    private void ResetCounter()
    {
        isCounting = false;
        clickCount = 0;
    }
}
