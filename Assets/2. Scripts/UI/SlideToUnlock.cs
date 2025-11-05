using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Slider))]
[RequireComponent(typeof(CanvasGroup))] 
public class SlideToUnlock : MonoBehaviour, IPointerUpHandler
{
    [SerializeField] private UIPasswordPanel passwordPanel;

    private Slider slider;
    private CanvasGroup canvasGroup;

    private float disabledAlpha = 100f / 255f;
    private float enabledAlpha = 1.0f;

    void Awake()
    {
        slider = GetComponent<Slider>();
        canvasGroup = GetComponent<CanvasGroup>(); 
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!slider.interactable) return;

        if (slider.value == slider.maxValue)
        {
            passwordPanel.OnPasswordSubmitButtonClicked();
        }
        else
        {
            slider.value = slider.minValue;
        }
    }

    private void OnDisable()
    {
        slider.value = slider.minValue;
    }

    public void SetReadyState(bool isReady)
    {
        if (isReady)
        {
            canvasGroup.alpha = enabledAlpha;
            slider.interactable = true;
        }
        else
        {
            canvasGroup.alpha = disabledAlpha;
            slider.interactable = false;
            slider.value = slider.minValue;
        }
    }
    public void ResetValue()
    {
        slider.value = slider.minValue;
    }
}