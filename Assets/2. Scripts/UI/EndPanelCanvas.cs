using UnityEngine;
using UnityEngine.UI;

public class EndPanelCanvas : MonoBehaviour
{
    public Button TitleBtn;
    public Button ExitBtn;

    private void OnEnable()
    {
        TitleBtn.onClick.AddListener(On_Title_Button_Clicked);
        ExitBtn.onClick.AddListener(On_Exit_Button_Clicked);
    }

    private void On_Title_Button_Clicked()
    {
        SoundManager.Instance.Play_Sfx(SFX.Click);

        this.gameObject.SetActive(false);

        SceneLoadManager.Instance.LoadScene(SceneType.Title);
    }

    private void On_Exit_Button_Clicked()
    {
        SoundManager.Instance.Play_Sfx(SFX.Click);

        Application.Quit();
    }

    private void OnDisable()
    {
        TitleBtn.onClick.RemoveListener(On_Title_Button_Clicked);
        ExitBtn.onClick.RemoveListener(On_Exit_Button_Clicked);
    }
}
