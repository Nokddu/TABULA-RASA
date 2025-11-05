using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cutscene_Play : MonoBehaviour
{
    public string ImgPath;
    public float PlayTime;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Show_Tutorial.Show_CutScene(ImgPath, PlayTime);
    }
}
