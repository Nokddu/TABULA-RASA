using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro;

public class ButtonHoverColor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("적용 대상")]
    public Image targetImage;
    public TextMeshProUGUI targetText;

    [Header("색상 설정")]
    public Color imageNormalColor = Color.black;
    public Color imageHighlightColor = Color.white;
    public Color textNormalColor = Color.white;
    public Color textHighlightColor = Color.black;

    [Header("속도 설정")]
    public float fadeDuration = 0.2f;

    private Tween imageTween;
    private Tween textTween;

    void OnEnable()
    {
        if (targetImage == null) targetImage = GetComponent<Image>();
        if (targetText == null) targetText = GetComponentInChildren<TextMeshProUGUI>();

        targetImage.color = imageNormalColor;
        if (targetText != null)
            targetText.color = textNormalColor;
    }
    void OnDisable()
    {
        imageTween?.Kill();
        textTween?.Kill();
        targetImage.color = imageNormalColor;
        if (targetText != null)
            targetText.color = textNormalColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        imageTween?.Kill();
        textTween?.Kill();

        imageTween = targetImage.DOColor(imageHighlightColor, fadeDuration)
                                .SetEase(Ease.OutQuad);
        if (targetText != null)
            textTween = targetText.DOColor(textHighlightColor, fadeDuration)
                                  .SetEase(Ease.OutQuad);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        imageTween?.Kill();
        textTween?.Kill();

        imageTween = targetImage.DOColor(imageNormalColor, fadeDuration)
                                .SetEase(Ease.OutQuad);
        if (targetText != null)
            textTween = targetText.DOColor(textNormalColor, fadeDuration)
                                  .SetEase(Ease.OutQuad);
    }
}