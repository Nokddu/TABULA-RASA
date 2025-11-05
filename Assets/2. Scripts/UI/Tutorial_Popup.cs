using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class Tutorial_Popup : MonoBehaviour
{
    public TextMeshProUGUI Key_Description;
    public TextMeshProUGUI InputKey;
    public VideoPlayer Video;

    public void Settting(string Key,string desctiption,string imagePath)
    {
        Key_Description.text = desctiption;
        InputKey.text = Key;
        Video.clip = Resources.Load<VideoClip>(imagePath);
        StartCoroutine(WaitDialog());
    }
    IEnumerator WaitDialog()
    {
        yield return new WaitForSeconds(0.2f);

        GameManager.Instance.Player.InteractOn(Vector2.right, true, true, true);
    }

    public void Exit()
    {
        Destroy(gameObject);
        GameManager.Instance.Player.InteractOff();
    }
}
