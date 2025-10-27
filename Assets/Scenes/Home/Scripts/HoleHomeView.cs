using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HoleHomeView : MonoBehaviour
{
    public int plusLevel;

    public TextMeshProUGUI[] txtInfo;

    public GameObject[] contents;

    void Start()
    {
        int curLevel = GameManager.Instance.GetMasterData().dataStage.Get() + plusLevel;

        foreach (var item in txtInfo)
        {
            item.text = string.Format("{0}", curLevel);
        }

        foreach (var item in contents)
        {
            item.SetActive(false);
        }
        
        contents[GameManager.Instance.GetStageModelView().GetTypeLevelLoop(plusLevel)].SetActive(true);
    }
}
