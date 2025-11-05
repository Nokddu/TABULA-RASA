using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Show_Tutorial : MonoBehaviour
{
    public static void Show_TutorialPopUp(string key, string text, string imgPath)
    {
        Instantiate(Resources.Load<GameObject>("Prefabs/UI/Tutorial_PopUp")).GetComponent<Tutorial_Popup>().Settting(key, text, imgPath);
    }

    public static void Show_CutScene(string imgPath, float playtime)
    {
        Instantiate(Resources.Load<GameObject>("Prefabs/UI/CutScene")).GetComponent<CutScene>().Settting(imgPath, playtime);
    }
}
