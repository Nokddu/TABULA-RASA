using UnityEngine;
using TMPro;
public class TooltipManager : MonoBehaviour
{
    public static TooltipManager Instance { get; private set; }

    [SerializeField] private RectTransform tooltipRectTransform;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI sizeText;
    [SerializeField] private TextMeshProUGUI rKeyText;
    [SerializeField] private Vector2 offset = new Vector2(15f, -15f);

    [SerializeField] private Texture2D tooltipCursorTexture;
    [SerializeField] private Vector2 cursorHotspot = Vector2.zero;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            gameObject.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (gameObject.activeSelf)
        {
            tooltipRectTransform.position = (Vector2)Input.mousePosition + offset;
        }
    }

    /// <summary>
    /// 툴팁을 보여줍니다.
    /// </summary>
    /// <param name="text">툴팁에 표시될 텍스트</param>
    public void ShowTooltip(string name, string description, int width, int height)
    {
        gameObject.SetActive(true);
        nameText.text = name;

        if (!string.IsNullOrEmpty(description))
        {
            descriptionText.gameObject.SetActive(true);
            descriptionText.text = description;
        }
        else
        {
            descriptionText.gameObject.SetActive(false);
        }

        if (sizeText != null)
        {
            if (width > 0 && height > 0)
            {
                sizeText.gameObject.SetActive(true);
                sizeText.text = $"[ {width} x {height} ]";
            }

            else
            {
                sizeText.gameObject.SetActive(false);
            }
        }

        if (rKeyText != null)
        {
            rKeyText.gameObject.SetActive(true);
            rKeyText.text = "좌클릭 후 [R]키를 누르면 회전";
        }

        if (tooltipCursorTexture != null)
        {
            Cursor.SetCursor(tooltipCursorTexture, cursorHotspot, CursorMode.Auto);
        }
    }
    public void ShowTooltip(string name, string description)
    {
        ShowTooltip(name, description, 0, 0);
    }

    public void ShowTooltip(string name, int width, int height)
    {
        gameObject.SetActive(true);

        if (nameText != null)
        {
            if (width > 0 && height > 0)
            {
                string sizeColorHex = "FFFFFF";
                nameText.text = $"<color=#{sizeColorHex}>[ {width} x {height} ]</color> {name}";
            }
            else
            {
                nameText.text = name;
            }
        }

        if (descriptionText != null) descriptionText.gameObject.SetActive(false);
        if (sizeText != null) sizeText.gameObject.SetActive(false);

        if (rKeyText != null)
        {
            rKeyText.gameObject.SetActive(false);
        }

    }
    public void HideTooltip()
    {
        gameObject.SetActive(false);

        if (nameText != null)
        {
            nameText.text = string.Empty;
        }

        if (descriptionText != null)
        {
            descriptionText.text = string.Empty;
        }

        if (sizeText != null)
        {
            sizeText.text = string.Empty;
        }

        if (rKeyText != null)
        {
            rKeyText.text = string.Empty;
        }

        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}