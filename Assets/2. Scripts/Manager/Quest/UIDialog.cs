using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIDialog : MonoBehaviour
{
    private DialogManager _dialogManager;
    private DataManager _dataManager;

    [SerializeField] private TMP_Text _speakerName;
    [SerializeField] private TMP_Text _dialogText;
    [SerializeField] private Button _acceptButton;
    [SerializeField] private TMP_Text _acceptText;
    [SerializeField] private Button _rejectButton;
    [SerializeField] private TMP_Text _rejectText;
    [SerializeField] public Button _cancelButton;
    [SerializeField] private GameObject _panel;
    [SerializeField] private GameObject _logPanel;
    [SerializeField] private Image _portraitImage1;
    [SerializeField] private Image _portraitImage2;
    [SerializeField] private GameObject _portrait1;
    [SerializeField] private GameObject _portrait2;
    [SerializeField] private Image _background;

    [SerializeField] private Color _acceptNormalColor = Color.white;
    [SerializeField] private Color _acceptDisabledColor = new Color(0.7f, 0.7f, 0.7f, 0.5f);

    [SerializeField] private float _typingSpeed;

    private Color _portrait1Color;
    private Color _portrait2Color;

    private string _dialogFullText;

    private static Action _offPanel;

    void Start()
    {
        _offPanel += InActivePanel;
        _dialogManager = DialogManager.Instance;
        _dataManager = DataManager.Instance;
    }
    private void OnDestroy()
    {
        _offPanel -= InActivePanel;
    }

    public void SetSpeaker(string Name)
    {
        _speakerName.text = Name;
    }

    public IEnumerator SetDialogTextGradually(string DialogText)
    {
        _dialogFullText = DialogText;

        _dialogText.text = "";
        foreach (char letter in DialogText.ToCharArray())
        {
            _dialogText.text += letter;
            yield return new WaitForSeconds(_typingSpeed);

            if (Input.GetMouseButton(0))
            {
                _dialogManager.FullTextCall = true;
                SetDialogText();
                break;
            }
        }
    }

    public void SetDialogText()
    {
        if (!_dialogFullText.Equals(""))
        {
            _dialogText.text = _dialogFullText;
        }
    }

    public void SetButton3(int Id)
    {
        if (!DataManager.Instance.DialogNodes.ContainsKey(Id))
        {
            Debug.Log($"DialogNodes 딕셔너리에 ID'{Id}'가 존재하지 않음. 데이터 파일 확인 필요");
            _acceptButton.onClick.RemoveAllListeners(); 
            _acceptButton.interactable = false;
            return;
        }

        _acceptButton.onClick.RemoveAllListeners();

        DialogNode TargetDialogNode = DataManager.Instance.DialogNodes[Id];
        _acceptButton.onClick.AddListener(() => DialogManager.Instance.PlayDialogNode(TargetDialogNode));
    }

    public void SetButton2(List<int> SelectButtonIdList, List<string> SelectButtonTextList)
    {
        _acceptButton.onClick.RemoveAllListeners();
        _rejectButton.onClick.RemoveAllListeners();

        // 버튼에 아이디 연결 
        if (SelectButtonIdList[0] > 0)
        {
            DialogNode TargetDialogNode = DataManager.Instance.DialogNodes[SelectButtonIdList[0]];
            Debug.Log($"{TargetDialogNode.TargetDialogId}");
            _acceptButton.onClick.AddListener(() => DialogManager.Instance.PlayDialogNode(TargetDialogNode));
        }
        else
        {
            _acceptButton.onClick.AddListener(() => InActivePanel());
        }
        // 텍스트 세팅
        _acceptText.text = SelectButtonTextList[0];


        if (SelectButtonIdList[1] > 0)
        {
            DialogNode TargetDialogNode = DataManager.Instance.DialogNodes[SelectButtonIdList[1]];
            _rejectButton.onClick.AddListener(() => DialogManager.Instance.PlayDialogNode(TargetDialogNode));
        }
        else
        {
            _rejectButton.onClick.AddListener(() => InActivePanel());
        }
        _rejectText.text = SelectButtonTextList[1];
    }

    public void ActiveButton()
    {
        _acceptButton.gameObject.SetActive(true);
        _rejectButton.gameObject.SetActive(true);
    }

    public void InActiveButton()
    {
        _acceptButton.gameObject.SetActive(false);
        _rejectButton.gameObject.SetActive(false);
    }

    public void SetAcceptButtonState(bool IsEnabled, int SuccessDialogId, int FailDialogId)// 아이템 제출 버튼 UI 메서드
    {
        if (_acceptButton == null) return;

        _acceptButton.interactable = true;
        _acceptButton.onClick.RemoveAllListeners(); 

        if (IsEnabled)
        {
            if (!DataManager.Instance.DialogNodes.ContainsKey(SuccessDialogId))
            {
                Debug.LogError($"[UIDialog] Error: 성공 ID '{SuccessDialogId}'가 DialogNodes에 없습니다!");
                return;
            }

            if (_acceptButton.image != null)
                _acceptButton.image.color = _acceptNormalColor;

            DialogNode targetNode = DataManager.Instance.DialogNodes[SuccessDialogId];
            _acceptButton.onClick.AddListener(() => DialogManager.Instance.PlayDialogNode(targetNode));
        }
        else
        {
            if (!DataManager.Instance.DialogNodes.ContainsKey(FailDialogId))
            {
                Debug.LogError($"[UIDialog] Error: 실패 ID '{FailDialogId}'가 DialogNodes에 없습니다!");
                return;
            }

            if (_acceptButton.image != null)
                _acceptButton.image.color = _acceptDisabledColor;

            DialogNode targetNode = DataManager.Instance.DialogNodes[FailDialogId];
            _acceptButton.onClick.AddListener(() => DialogManager.Instance.PlayDialogNode(targetNode));
        }
    }
    public static void OffPanel()
    {
        _offPanel?.Invoke();
    }
    public void InActivePanel()
    {
        DialogManager.Instance.DialogLogList.Clear();
        _logPanel.SetActive(false); // 로그창 끄기.
        _panel.SetActive(false);
        _acceptButton.gameObject.SetActive(false);
        _rejectButton.gameObject.SetActive(false);
        GameManager.Instance.Player.InteractOff();
    }

    public void ActivePanel()
    {
        _panel.SetActive(true);
    }
    
    public void SetTypingSpeed(float TypingSpeed)
    {
        _typingSpeed = TypingSpeed;
    }

    private void ActivePortrait(int I)
    {
        switch (I)
        {
            // NPC1 초상화 오브젝트 활성화 
            case 0:
                _portrait1.SetActive(true);
                break;

            // NPC2 초상화 오브젝트 활성화 
            case 1:
                _portrait2.SetActive(true);
                break;
        }
    }

    private void InActivePortrait(int I)
    {
        switch(I)
        {
            // NPC1 초상화 오브젝트 비활성화 
            case 0:
                _portrait1.SetActive(false);
                break;

            // NPC2 초상화 오브젝트 비활성화 
            case 1:
                _portrait2.SetActive(false);
                break;
        }
    }

    private void SetPortraitImage(int I, int NpcId, int PortraitNumber)
    {
        switch (I)
        {
            // NPC1 초상화를 Portrait번호로 세팅 
            case 0:
                foreach(DialogImage DialogImage in DataManager.Instance.ImageNodes[NpcId])
                {
                    if (DialogImage.Value == PortraitNumber)
                    {
                        _portraitImage1.sprite = ResourceManager.Instance.LoadResource<Sprite>(DialogImage.Path); 
                        break;
                    }
                }
                break;

            // NPC2 초상화를 Portrait번호로 세팅 
            case 1:
                foreach(DialogImage DialogImage in DataManager.Instance.ImageNodes[NpcId])
                {
                    if(DialogImage.Value == PortraitNumber)
                    {
                        _portraitImage2.sprite = ResourceManager.Instance.LoadResource<Sprite>(DialogImage.Path);
                    }
                }
                break;
        }
    }

    private void SetPortraitAlpha(int ActiveNpc)
    {
        switch(ActiveNpc)
        {
            // NPC1을 강조해야 하면 
            case 1:
                _portrait1Color = _portraitImage1.color;
                _portrait1Color.a = 1.0f;
                _portraitImage1.color = _portrait1Color; // NPC1 초상화 알파값을 1로 설정 

                _portrait2Color = _portraitImage2.color;
                _portrait2Color.a = 0.5f;
                _portraitImage2.color = _portrait2Color;

                break;

            // NPC2를 강조해야 하면 
            case 2:
                _portrait2Color = _portraitImage2.color;
                _portrait2Color.a = 1.0f;
                _portraitImage2.color = _portrait2Color;

                _portrait1Color = _portraitImage1.color;
                _portrait1Color.a = 0.5f;
                _portraitImage1.color = _portrait1Color;

                break;

            // NPC1, NPC2 모두 강조해야 하면 
            case 3:
                _portrait1Color = _portraitImage1.color;
                _portrait1Color.a = 1.0f;
                _portraitImage1.color = _portrait1Color;

                _portrait2Color = _portraitImage2.color;
                _portrait2Color.a = 1.0f;
                _portraitImage2.color = _portrait2Color;

                break;
        }
    }

    // 초상화 세팅 
    public void SetPortrait(List<int> CurrentLineNpcList, List<int> CurrentLineNpcPortraitList, int NpcActive)
    {
        for(int i = 0; i < 2; i++)
        {
            // NPC가 있으면서 NPC 초상화가 있으면 
            if (CurrentLineNpcList[i] != -1 && CurrentLineNpcPortraitList[i] != -1)
            {
                ActivePortrait(i); 
                SetPortraitImage(i, CurrentLineNpcList[i], CurrentLineNpcPortraitList[i]); // i번째 초상화 이미지를 Portrait번호로 세팅
                SetPortraitAlpha(NpcActive); // NpcActive번째 초상화 강조/흐릿하게
            }

            // 초상화가 없으면
            else
            {
                InActivePortrait(i);
            }
        }
    }

    public void SetBackground(int BackgroundId)
    {
        if(BackgroundId != -1)
        {
            _background.gameObject.SetActive(true);
            _background.sprite = ResourceManager.Instance.LoadResource<Sprite>(_dataManager.ImageNodes[BackgroundId][0].Path);
        }
        else
        {
            _background.gameObject.SetActive(false);
        }
    }

    public void InActiveBackground()
    {
        _background.gameObject.SetActive(false);
    }

    public void SetCancelButton(int Id)
    {
        if (_cancelButton == null) return;
        if (!DataManager.Instance.DialogNodes.ContainsKey(Id))
        {
            return;
        }

        _cancelButton.gameObject.SetActive(true);
        _cancelButton.onClick.RemoveAllListeners();
        DialogNode targetNode = DataManager.Instance.DialogNodes[Id];
        _cancelButton.onClick.AddListener(() => DialogManager.Instance.PlayDialogNode(targetNode));
    }
}
