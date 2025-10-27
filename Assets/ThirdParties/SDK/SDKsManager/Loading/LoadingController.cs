using UnityEngine;

public class LoadingController : MonoBehaviour
{
    public GameObject objLoadingPanel;

    public Transform objLoadingIcon;

    private float rotateSpeed = 2;

    public void OnShow()
    {
        objLoadingPanel.SetActive(true);
    }

    public void OnHide()
    {
        objLoadingPanel.SetActive(false);
    }

    void Update()
    {
        if (objLoadingPanel != null && objLoadingPanel.activeInHierarchy)
        {
            objLoadingIcon.transform.Rotate(Vector3.back * rotateSpeed);
        }
    }
}
