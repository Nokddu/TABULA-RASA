using UnityEngine;

public class TopMenuController : MonoBehaviour
{
    // Unity 에디터에서 연결할 팝업 UI 오브젝트들
    public GameObject settingsPopup;
    public GameObject inventoryPopup;

    void Update()
    {
        // ESC 키를 누르면 설정창 토글
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleSettingsPopup();
        }

        // Tab 키를 누르면 인벤토리창 토글
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleInventoryPopup();
        }
    }

    // 설정창을 켜고 끄는 함수 (토글)
    public void ToggleSettingsPopup()
    {
        // 인벤토리창이 열려있으면 끈다
        if (inventoryPopup.activeSelf)
        {
            inventoryPopup.SetActive(false);
        }

        // 설정창의 현재 활성화 상태를 반전시킨다
        settingsPopup.SetActive(!settingsPopup.activeSelf);
    }

    // 인벤토리창을 켜고 끄는 함수 (토글)
    public void ToggleInventoryPopup()
    {
        // 설정창이 열려있으면 끈다
        if (settingsPopup.activeSelf)
        {
            settingsPopup.SetActive(false);
        }

        // 인벤토리창의 현재 활성화 상태를 반전시킨다
        inventoryPopup.SetActive(!inventoryPopup.activeSelf);
    }
}