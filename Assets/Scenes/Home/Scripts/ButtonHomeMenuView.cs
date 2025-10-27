using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ButtonHomeMenuView : MonoBehaviour
{
    public GameObject[] contents;

    void Start()
    {
        foreach (var item in contents)
        {
            item.SetActive(false);
        }
        
        contents[GameManager.Instance.GetStageModelView().GetTypeLevelLoop(0)].SetActive(true);
    }
}
