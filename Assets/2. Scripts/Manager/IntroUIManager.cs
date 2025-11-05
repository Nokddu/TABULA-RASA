using System.Collections;
using TMPro;
using UnityEngine;

public class IntroUIManager : MonoBehaviour
{
    [SerializeField] private GameObject SettingsButton; // 세팅 버튼
    [SerializeField] private GameObject ControlsButton; // 조작법 버튼
    [SerializeField] private GameObject ResolutionButton; // 해상도 버튼
    [SerializeField] private GameObject VolumeButton; // 볼륨 조절
    [SerializeField] private GameObject ExitPopupPanel; // 종료 버튼
    [SerializeField] private GameObject optionsPanel; // 옵션 목록 패널
    [SerializeField] private TextMeshProUGUI currentModeText; // 현재 모드를 표시할 텍스트
    [SerializeField] private RectTransform arrowIcon; // 회전시킬 화살표 아이콘
    [SerializeField] private GameObject newGameConfirmPopup; // 새 게임 확인 팝업
    [SerializeField] private TextMeshProUGUI newGamePopupText; // 새 게임 텍스트

    #region 해상도 관련 메서드
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
        SaveManager.Instance.SystemData.ScreenSize = "Full";
        SaveManager.Instance.Save_System(SaveManager.Instance.SystemData);
        CloseResolutionOptions();
    }

    public void OnSetBorderless() // 테두리 없는 창
    {
        Debug.Log("테두리 없는 창 화면");
        Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        UpdateResolutionText(Screen.fullScreenMode);
        SaveManager.Instance.SystemData.ScreenSize = "Borderless";
        SaveManager.Instance.Save_System(SaveManager.Instance.SystemData);
        CloseResolutionOptions();
    }

    public void OnSetWindow() // 창모드
    {
        Debug.Log("창 화면");
        Screen.fullScreenMode = FullScreenMode.Windowed;
        UpdateResolutionText(Screen.fullScreenMode);
        SaveManager.Instance.SystemData.ScreenSize = "Window";
        SaveManager.Instance.Save_System(SaveManager.Instance.SystemData);
        CloseResolutionOptions();
    }
    #endregion
    private void Awake()
    {
        Debug.Log($"Start 함수 시점의 화면 모드: {Screen.fullScreenMode}");

        Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        Debug.Log($"코드로 화면 모드를 FullScreen으로 변경 시도");
        UpdateResolutionText(Screen.fullScreenMode);
        Debug.Log($"UpdateResolutionText 실행 후 : '{currentModeText.text}'");
    }

    private IEnumerator Start()
    {
        newGameConfirmPopup?.SetActive(false);

        yield return new WaitUntil(() => SaveManager.Instance.IsLoadCompleted);

        string screenSize = SaveManager.Instance.SystemData.ScreenSize;

        FullScreenMode newFullScreenMode = screenSize switch
        {
            "Full" => FullScreenMode.ExclusiveFullScreen,
            "Borderless" => FullScreenMode.FullScreenWindow,
            _ => FullScreenMode.Windowed,
        };

        Screen.fullScreenMode = newFullScreenMode;
        UpdateResolutionText(newFullScreenMode);
    }

    private void DeactivateAllContentPanels()
    {
        ControlsButton?.SetActive(false);
        ResolutionButton?.SetActive(false);
        VolumeButton?.SetActive(false);
    } // 설정창의 모든 패널 비활성화

    public void OnControlsTabClicked()
    {
        DeactivateAllContentPanels();
        ControlsButton?.SetActive(true);
    } 

    public void OnResolutionTabClicked()
    {
        DeactivateAllContentPanels();
        ResolutionButton?.SetActive(true);
    }

    public void OnVolumeTabClicked()
    {
        DeactivateAllContentPanels();
        VolumeButton?.SetActive(true);
    }

    public void OnNewGameButtonClicked()
    {
        SoundManager.Instance.Play_Sfx(SFX.Click);

        if (SaveManager.Instance.IsLoaded)
        {
            ShowConfirmationPopup();
        }
        else
        {
            StartNewGame_Internal();
        }
    }

    public void OnContinueButtonClicked()
    {
        SoundManager.Instance.Play_Sfx(SFX.Click);

        UIManager.Instance.CurrentTxt.text = SaveManager.Instance.UserData.CurrentMilestone;

        SceneType scene = (SceneType)(SaveManager.Instance.UserData.SceneNumber + 1);

        SceneLoadManager.Instance.LoadScene(scene);
    }

    public void OnSettingsButtonClicked()
    {
        SoundManager.Instance.Play_Sfx(SFX.Click);

        if (SettingsButton != null)
        {
            SettingsButton.SetActive(true);
            OnControlsTabClicked();
        }
    }

    public void OnCloseSettingsPanelClicked()
    {
        SoundManager.Instance.Play_Sfx(SFX.Click);

        if (SettingsButton != null)
        {
            SettingsButton.SetActive(false);
        }
    }

    public void OnExitGameButtonClicked()
    {
        SoundManager.Instance.Play_Sfx(SFX.Click);

        ExitPopupPanel?.SetActive(true);
    }
    public void OnYesQuitButtonClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    } 

    public void OnNoExitButtonClicked()
    {
        ExitPopupPanel?.SetActive(false);
    }

    private void StartNewGame_Internal()
    {
        if (SaveManager.Instance.IsLoaded)
        {
            SaveManager.Instance.ReLoad_Data();
        }

        UIManager.Instance.CurrentTxt.text = SaveManager.Instance.UserData.CurrentMilestone;
        SceneLoadManager.Instance.LoadScene(SceneType.Tutorial);
    }
    public void OnClick_ConfirmNewGame_Yes()
    {
        SoundManager.Instance.Play_Sfx(SFX.Click);
        newGameConfirmPopup.SetActive(false);
        StartNewGame_Internal(); // 데이터를 리셋하고 씬을 로드
    }
    public void OnClick_ConfirmNewGame_No()
    {
        SoundManager.Instance.Play_Sfx(SFX.Click);
        newGameConfirmPopup.SetActive(false);
    }

    private void ShowConfirmationPopup()
    {
        newGameConfirmPopup?.SetActive(true);
    }
}