using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    private Stack<UIBase> _popupStack = new Stack<UIBase>();

    [Header("Current_Text")]
    public TextMeshProUGUI CurrentTxt;

    [Header("UI Prefabs & Canvas")]
    [SerializeField] private Transform canvasTransform;
    [SerializeField] private UIBase menuPopupPrefab;


    #region 싱글턴 및 초기화
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion
    // ESC 키 입력 처리 로직
    public void HandleEscapeInput()
    {
        if (!GameManager.Instance.Player.IsTalking())
        {
            SoundManager.Instance.Play_Sfx(SFX.Menu);

            // 인벤토리가 열려있으면 닫음.
            if (InventoryManager.Instance.IsInventory)
            {
                InventoryManager.Instance.HideAllInventories();

            }
            // 다른 팝업(메뉴 등)이 열려있으면 닫음.
            else if (_popupStack.Count > 0)
            {
                CloseCurrentPopup();
                GameManager.Instance.Player.InteractOff();
            }
            // 아무것도 열려있지 않으면 메뉴를 염.
            else
            {
                ShowPopup(menuPopupPrefab);
                GameManager.Instance.Player.InteractOn(Vector2.down, true, true, false);
            }
        }
    }
    public void ShowPopup(UIBase popupPrefab)
    {
        if (popupPrefab == null)
        { 
            return; 
        }
        foreach (var openPopup in _popupStack) 
        { 
            if (openPopup.GetType() == popupPrefab.GetType()) 
                return; 
        }

        UIBase uiInstance = Instantiate(popupPrefab, canvasTransform);
        uiInstance.Init();
        uiInstance.OnCloseAnimationEnd += () => HandlePopupClose(uiInstance);
        _popupStack.Push(uiInstance);
        uiInstance.Open();
    }

    public void CloseCurrentPopup()
    {
        if (_popupStack.Count > 0)
        {
            UIBase topPopup = _popupStack.Peek();
            topPopup.Close();
        }
    }

    private void HandlePopupClose(UIBase ui)
    {
        if (_popupStack.Count > 0 && _popupStack.Peek() == ui)
        {
            _popupStack.Pop();
            Destroy(ui.gameObject);
        }
    }

    // 버튼 클릭을 위한 Public 함수 이름 변경
    public void OnClick_MenuButton()
    {
        HandleEscapeInput();
    }
}