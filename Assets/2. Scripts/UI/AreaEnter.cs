using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaEnter : MonoBehaviour
{
    public AreaManager area;

    public string AreaName;

    [SerializeField]
    private Ambience AmbienceSFX;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(area.FadeIn(area.fadeInDuration, area.stayDuration, area.fadeOutDuration, AreaName));

            ObjectPool.Instance.Get_Pool_Ambience($"{AmbienceSFX}", AmbienceSFX);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameObject ambienceToReturn = SoundManager.Instance.Remove_Ambience($"{AmbienceSFX}");

            ObjectPool.Instance.Return_To_Ambience($"{AmbienceSFX}", ambienceToReturn);
        }
    }
}
