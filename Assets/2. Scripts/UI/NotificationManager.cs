using UnityEngine;
using System.Collections;
using TMPro;

public class NotificationManager : Singleton<NotificationManager>
{
    [Header("알림 패널")]
    public GameObject ghostMessagePanel;
    public GameObject inventoryFullPanel;
    public GameObject cannotDropPanel;

    [Header("표시 설정")]
    public float displayTime = 2.0f;

    private Coroutine _ghostCoroutine;
    private Coroutine _inventoryFullCoroutine;
    private Coroutine _cannotDropCoroutine;

    public void ShowGhostMessage()
    {
        if (_ghostCoroutine != null)
        {
            StopCoroutine(_ghostCoroutine);
        }
        _ghostCoroutine = StartCoroutine(ShowAndHidePanel(ghostMessagePanel, _ghostCoroutine));
    }
    public void ShowInventoryFullMessage()
    {
        if (_inventoryFullCoroutine != null)
        {
            StopCoroutine(_inventoryFullCoroutine);
        }
        _inventoryFullCoroutine = StartCoroutine(ShowAndHidePanel(inventoryFullPanel, _inventoryFullCoroutine));
    }
    public void ShowCannotDropMessage()
    {
        if (_cannotDropCoroutine != null)
        {
            StopCoroutine(_cannotDropCoroutine);
        }
        _cannotDropCoroutine = StartCoroutine(ShowAndHidePanel(cannotDropPanel, _cannotDropCoroutine));
    }
    private IEnumerator ShowAndHidePanel(GameObject panel, Coroutine coroutineVariable)
    {
        if (panel == null)
        {
            yield break;
        }

        panel.SetActive(true);
        yield return new WaitForSeconds(displayTime);
        panel.SetActive(false);

        if (coroutineVariable == _ghostCoroutine) _ghostCoroutine = null;
        else if (coroutineVariable == _inventoryFullCoroutine) _inventoryFullCoroutine = null;
        else if (coroutineVariable == _cannotDropCoroutine) _cannotDropCoroutine = null;
    }
}