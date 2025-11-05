using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI; 

public class UIScriptLog : MonoBehaviour
{
    [SerializeField] private TMP_Text _logText;
    [SerializeField] private ScrollRect _scrollRect; 
    private StringBuilder _strBuilder = new StringBuilder();

    private bool flag = false;
    private int _lastLogCount = 0; 

    private void Awake()
    {
        if (_scrollRect == null)
        {
            _scrollRect = GetComponent<ScrollRect>();
        }
    }

    private void Update()
    {
        if (flag)
        {
            int currentLogCount = DialogManager.Instance.DialogLogList.Count;

            if (currentLogCount != _lastLogCount)
            {
                BuildLog(); 
            }
        }
    }
    public void ShowDialogLog()
    {
        if (!flag) 
        {
            flag = true;
            this.gameObject.SetActive(true);
            BuildLog(); 
        }
        else 
        {
            CloseLogPanel(); 
        }
    }

    public void CloseLogPanel()
    {
        if (flag)
        {
            flag = false;
            this.gameObject.SetActive(false);

            _logText.text = string.Empty;
            _strBuilder.Clear();
            _lastLogCount = 0; 
        }
    }
    private void BuildLog()
    {
        List<string> DialogList = DialogManager.Instance.DialogLogList;
        _strBuilder.Clear(); 

        foreach (string script in DialogList)
        {
            _strBuilder.Append($"{script}\n\n\n");
        }

        _logText.text = _strBuilder.ToString();
        _lastLogCount = DialogList.Count; 

        if (_scrollRect != null)
        {
            RectTransform contentRect = _scrollRect.content;
            LayoutRebuilder.ForceRebuildLayoutImmediate(contentRect);

            _scrollRect.verticalNormalizedPosition = 0f;
        }
    }
}