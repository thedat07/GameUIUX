using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Game/SoTextGame", order = 1)]
public class SoTextGame : ScriptableObject
{
    [TextArea]
    public string[] lstText;

    public string GetText(int index)
    {
        if (index < lstText.Length && index >= 0)
        {
            return lstText[index];
        }
        else
        {
            return "";
        }
    }
}
