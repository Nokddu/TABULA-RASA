using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class CutScene : MonoBehaviour
{
    public VideoPlayer Video;

    public void Settting(string imagePath, float playtime)
    {
        Video.clip = Resources.Load<VideoClip>(imagePath);
        StartCoroutine(WaitDialog(playtime));
    }

    IEnumerator WaitDialog(float playtime)
    {
        GameManager.Instance.Player.InteractOn(Vector2.right, true, true, true);

        yield return new WaitForSeconds(playtime);

        Destroy(gameObject);
    }
}
