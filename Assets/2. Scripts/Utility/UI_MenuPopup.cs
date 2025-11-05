using TMPro;
using UnityEngine;

public class UI_MenuPopup : UIBase
{
    // 메뉴 팝업에만 필요한 특별한 기능이 있다면 여기에 추가합니다.
    // 예: public void OnClick_SettingsButton() { ... }
    [SerializeField] private GameObject optionsPanel; // 옵션 목록 패널
    [SerializeField] private RectTransform arrowIcon; // 회전시킬 화살표 아이콘
    [SerializeField] private TextMeshProUGUI currentModeText; // 현재 모드를 표시할 텍스트

    public void OnResolutionDropdownClicked()
    {
        bool isActive = !optionsPanel.activeSelf;
        optionsPanel.SetActive(isActive);
        arrowIcon.localRotation = Quaternion.Euler(0, 0, isActive ? 180f : 0f);
    }
    private void CloseResolutionOptions()
    {
        optionsPanel.SetActive(false);
        arrowIcon.localRotation = Quaternion.Euler(0, 0, 0f);
    }
    void UpdateResolutionText(FullScreenMode mode)
    {
        switch (mode)
        {
            case FullScreenMode.ExclusiveFullScreen:
                currentModeText.text = "Full";
                break;
            case FullScreenMode.FullScreenWindow:
                currentModeText.text = "Borderless";
                break;
            case FullScreenMode.Windowed:
                currentModeText.text = "Window";
                break;
            default:
                currentModeText.text = "Full";
                break;
        }
    }

    public void OnSetFullscreen() //전체화면
    {
        Debug.Log("전체화면");
        Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        UpdateResolutionText(Screen.fullScreenMode);
        CloseResolutionOptions();
    }

    public void OnSetWindow() // 창모드
    {
        Debug.Log("창 화면");
        Screen.fullScreenMode = FullScreenMode.Windowed;
        UpdateResolutionText(Screen.fullScreenMode);
        CloseResolutionOptions();
    }

    public void OnSetBorderless() // 테두리 없는 창
    {
        Debug.Log("테두리 없는 창 화면");
        Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        UpdateResolutionText(Screen.fullScreenMode);
        CloseResolutionOptions();
    }

    public void MainMenu()
    {
        SceneLoadManager.Instance.LoadScene(SceneType.Title);

        SaveManager.Instance.Load_Data();

        UIManager.Instance.HandleEscapeInput();
    }

    public void QuitGame()
    {
        SaveManager.Instance.Load_Data();

        Application.Quit();
    }
}