using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TabButton : MonoBehaviour, IPointerClickHandler
{
    [Header("연결할 콘텐츠 패널")]
    public GameObject contentPanel;

    [HideInInspector]
    public TabGroup tabGroup;

    private Image _buttonImage;
    public Image SelectButtonImg;
    void Awake()
    {
        _buttonImage = GetComponent<Image>();
    }

    public void Select()
    {
        SoundManager.Instance.Play_Sfx(SFX.Click);

        if (contentPanel != null)
        {
            contentPanel.SetActive(true);
            SelectButtonImg.enabled = true;
        }

        if (tabGroup != null && _buttonImage != null)
        {
            _buttonImage.color = tabGroup.activeTabColor;
        }
    }
    public void Deselect()
    {
        if (contentPanel != null)
        {
            contentPanel.SetActive(false);
            SelectButtonImg.enabled = false;
        }

        if (tabGroup != null && _buttonImage != null)
        {
            _buttonImage.color = tabGroup.inactiveTabColor;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        tabGroup.OnTabSelected(this);
    }
}