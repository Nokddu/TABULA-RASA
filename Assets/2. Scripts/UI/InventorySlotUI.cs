using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    public int GridX { get; private set; }
    public int GridY { get; private set; }

    [Header("UI References")]
    public Image itemIcon;
    public Image previewImage;
    public Image availabilityOverlay;
    private Outline slotOutline;

    public InventoryUI inventoryUI;
    private int currentSlotSize;

    private Canvas itemCanvas;
    private GraphicRaycaster graphicRaycaster;

    private bool isAvailable = true;

    private InventoryItem _currentItem;

    void Awake()
    {
        slotOutline = GetComponent<Outline>();
    }

    public void SetSlot(int x, int y, InventoryUI invUI, int slotSize)
    {
        GridX = x;
        GridY = y;
        inventoryUI = invUI;
        currentSlotSize = slotSize;
    }

    public void UpdateItem(InventoryItem item)
    {
        _currentItem = item;

        bool isAnchorSlot = (item != null && item.GridX == GridX && item.GridY == GridY);
        itemIcon.gameObject.SetActive(isAnchorSlot);

        if (isAnchorSlot)
        {
            Sprite loadedSprite = Resources.Load<Sprite>(item.ItemData.ImagePath);
            if (loadedSprite != null)
            {
                itemIcon.sprite = loadedSprite;
            }
            else
            {
                Debug.Log($"'{item.ItemData.ImagePath}' 경로에서 이미지를 불러오지 못했습니다!");
            }
            itemIcon.rectTransform.sizeDelta = new Vector2(item.width * currentSlotSize, item.height * currentSlotSize);

            if (itemCanvas == null)
            {
                itemCanvas = gameObject.AddComponent<Canvas>();
                graphicRaycaster = gameObject.AddComponent<GraphicRaycaster>();
                itemCanvas.overrideSorting = true;
                itemCanvas.sortingOrder = 1;
            }
        }
        else
        {
            if (itemCanvas != null)
            {
                // [핵심 수정] 의존하는 컴포넌트(GraphicRaycaster)를 먼저 제거해야 합니다.
                Destroy(graphicRaycaster);
                Destroy(itemCanvas);
            }
        }
    }

    public void SetAvailability(bool available)
    {
        isAvailable = available;

        availabilityOverlay.gameObject.SetActive(!isAvailable);
        if (!isAvailable)
        {
            availabilityOverlay.color = new Color(0.2f, 0.2f, 0.2f, 0.7f);
        }

        if (slotOutline != null)
        {
            slotOutline.enabled = !isAvailable;
        }
    }

    public void SetPreviewColor(Color color)
    {
        previewImage.color = color;
        previewImage.gameObject.SetActive(true);
    }

    public void ResetPreviewColor()
    {
        previewImage.gameObject.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isAvailable) return;

        if (TooltipManager.Instance != null)
        {
            TooltipManager.Instance.HideTooltip();
        }

        inventoryUI.OnSlotPointerDown(GridX, GridY);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (TooltipManager.Instance != null && _currentItem != null && InventoryManager.Instance.CurrentDraggedItem == null)
        {
            TooltipManager.Instance.ShowTooltip(_currentItem.ItemData.Name,_currentItem.ItemData.Description);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (TooltipManager.Instance != null)
        {
            TooltipManager.Instance.HideTooltip();
        }
    }
}