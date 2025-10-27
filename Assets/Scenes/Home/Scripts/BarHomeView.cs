using System.Collections;
using System.Collections.Generic;
using DanielLochner.Assets.SimpleScrollSnap;
using UnityEngine;

public class BarHomeView : MonoBehaviour
{
    public ButtonHomeBar[] toggles;

    public void GoToPanel(int item1, int item2)
    {
        toggles[item1].TurnOn();
    }
}
