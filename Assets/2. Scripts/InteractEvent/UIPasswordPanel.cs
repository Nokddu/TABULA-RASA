using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(CanvasGroup))]
public class UIPasswordPanel : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Button[] _passwordButtons;
    [SerializeField] private Password _passwordNpc;
    [SerializeField] private TMP_Text _passwordDisplay;
    [SerializeField] private SlideToUnlock unlockSlider;

    [Header("Effect Settings")]
    [SerializeField] private float fadeDuration = 0.3f;
    [SerializeField] private float checkDelay = 2.0f;

    [Header("Color Settings")]
    [SerializeField] private Color successColor = new Color(0.2f, 0.4f, 1f);
    [SerializeField] private Color failColor = new Color(1f, 0.3f, 0.2f);

    public string Password { get; set; }
    private CanvasGroup canvasGroup;

    private Image panelBackground;
    private Color originalPanelColor;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        panelBackground = GetComponent<Image>();
        if (panelBackground != null)
        {
            originalPanelColor = panelBackground.color;
        }

        for (int i = 0; i < _passwordButtons.Length; i++)
        {
            string buttonValue = _passwordButtons[i].name;
            _passwordButtons[i].onClick.AddListener(() => OnPasswordButtonClick(buttonValue));
        }
    }
    private void Start()
    {
        if (unlockSlider != null)
        {
            unlockSlider.SetReadyState(false);
        }
    }

    private void OnEnable()
    {
        Password = string.Empty;
        UpdatePasswordDisplay(false);

        if (_passwordDisplay != null)
        {
            _passwordDisplay.color = Color.green; 
        }

        if (panelBackground != null)
        {
            panelBackground.color = originalPanelColor;
        }

        StartCoroutine(Fade(true));
    }

    public void OnPasswordButtonClick(string inputChar)
    {
        SoundManager.Instance.Play_Sfx(SFX.DoorClick);
        if (Password.Length < 4)
        {
            Password += inputChar;
            UpdatePasswordDisplay(true);
        }

        if (Password.Length == 4)
        {
            if (unlockSlider != null)
                unlockSlider.SetReadyState(true);
        }
    }

    public void OnPasswordSubmitButtonClicked()
    {
        StartCoroutine(SubmitRoutine());
    }

    public void OnPasswordCancelButtonClicked()
    {
        StartCoroutine(Fade(false));
    }

    public void ResetPassword()
    {
        Password = string.Empty;
        UpdatePasswordDisplay(true);

        if (unlockSlider != null)
        {
            unlockSlider.SetReadyState(false);
            unlockSlider.ResetValue();
        }

        if (panelBackground != null)
        {
            panelBackground.color = originalPanelColor;
        }
    }

    private IEnumerator SubmitRoutine()
    {
        canvasGroup.interactable = false;

        float timer = 0f;
        float animationInterval = 0.4f;
        string baseText = "확인중";

        while (timer < checkDelay)
        {
            int dotCount = (int)(timer / animationInterval) % 3 + 1;
            _passwordDisplay.text = baseText + new string('.', dotCount);
            yield return null;
            timer += Time.deltaTime;
        }

        _passwordDisplay.text = "확인중...";

        bool isCorrect = _passwordNpc.CheckPassword(Password);

        if (isCorrect)
        {
            _passwordDisplay.text = "Success"; 
            if (panelBackground != null)
                panelBackground.color = successColor;

            _passwordNpc.TriggerSuccess();

            yield return new WaitForSeconds(1.0f);
            StartCoroutine(Fade(false));
        }
        else
        {
            _passwordDisplay.text = "Fail"; 
            if (panelBackground != null)
                panelBackground.color = failColor; 

            yield return new WaitForSeconds(1.0f);

            ResetPassword();
            _passwordNpc.TriggerFailure();

            canvasGroup.interactable = true;
        }
    }

    private IEnumerator Fade(bool fadeIn)
    {
        canvasGroup.interactable = false;
        float startAlpha = fadeIn ? 0f : 1f;
        float endAlpha = fadeIn ? 1f : 0f;
        float timer = 0f;
        canvasGroup.alpha = startAlpha;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, timer / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = endAlpha;

        if (!fadeIn)
        {
            gameObject.SetActive(false);

            if (GameManager.Instance != null && GameManager.Instance.Player != null)
            {
                GameManager.Instance.Player.InteractOff();
            }
        }

        canvasGroup.interactable = true;
    }

    private void UpdatePasswordDisplay(bool useSpacing)
    {
        if (_passwordDisplay == null) return;

        string maskedText;

        if (string.IsNullOrEmpty(Password))
        {
            maskedText = string.Empty;
        }
        else if (Password.Length == 1)
        {
            maskedText = Password;
        }
        else
        {
            string asterisks = new string('*', Password.Length - 1);
            char lastChar = Password[Password.Length - 1];
            maskedText = asterisks + lastChar;
        }

        if (string.IsNullOrEmpty(maskedText))
        {
            _passwordDisplay.text = string.Empty;
        }
        else if (useSpacing)
        {
            _passwordDisplay.text = string.Join(" ", maskedText.ToCharArray());
        }
        else
        {
            _passwordDisplay.text = maskedText;
        }
    }
}