using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using DG.Tweening;
public enum InventoryAnimType
{
    ScaleFade,
    SlideLeft,
    PopUpBottom,
    FadeOnly
}

public class InventoryUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Inventory inventory;
    public bool IsInitialized { get; private set; }
    public bool IsMouseOverUI { get; private set; } = false;

    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    public float animationDuration = 0.5f;

    private bool _isQuestOfferingBox = false;
    private List<int> _allowedItemIDs;

    [Header("Grid Settings")]
    public int gridWidth = 4;
    public int gridHeight = 4;
    public int usableWidth = 4;
    public int usableHeight = 4;

    [Header("UI References")]
    public GameObject slotPrefab;
    public Transform gridLayoutGroup;

    [Header("Effects")]
    public Image backgroundPanelImage;
    public float pulseSpeed = 1f;
    public float minAlpha = 0.8f;
    public float maxAlpha = 1.0f;
    private bool isPulsating = false;

    private List<InventorySlotUI> slotUIList = new List<InventorySlotUI>();
    private int slotSize = 100;

    private Vector3 originalScale;

    void Awake()
    {
        CreateGridSlots();
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();

        originalScale = transform.localScale;

        gameObject.SetActive(false);
    }

    void Update()
    {
        if (InventoryManager.Instance.CurrentDraggedItem != null && InventoryManager.Instance.SourceInventoryUI == this)
        {
            if (Input.GetMouseButtonUp(0))
            {
                HandleItemDrop();
                return;
            }
            UpdatePlacementPreview(GetSlotUnderMouse());
        }
        if (isPulsating && backgroundPanelImage != null)
        {
            float sineValue = (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f;
            float targetAlpha = Mathf.Lerp(minAlpha, maxAlpha, sineValue);
            var color = backgroundPanelImage.color;
            backgroundPanelImage.color = new Color(color.r, color.g, color.b, targetAlpha);
        }
    }

    public void OnPointerEnter(PointerEventData eventData) { IsMouseOverUI = true; }
    public void OnPointerExit(PointerEventData eventData) { IsMouseOverUI = false; }

    private void HandleItemDrop()
    {
        InventoryItem droppedItem = InventoryManager.Instance.CurrentDraggedItem;
        if (droppedItem == null) return;

        InventorySlotUI dropSlot = GetSlotUnderMouse();
        bool placedSuccessfully = false;

        if (dropSlot != null)
        {
            InventoryUI targetInventoryUI = dropSlot.inventoryUI;
            if (targetInventoryUI._isQuestOfferingBox && !targetInventoryUI._allowedItemIDs.Contains(droppedItem.ItemData.Id))
            {
                placedSuccessfully = false;
            }
            else
            {
                if (targetInventoryUI != this)
                {
                    placedSuccessfully = InventoryManager.Instance.TransferItem(droppedItem, this.inventory, targetInventoryUI.GetInventory(), dropSlot.GridX, dropSlot.GridY);
                }
                else
                {
                    placedSuccessfully = inventory.PlaceItem(droppedItem, dropSlot.GridX, dropSlot.GridY);
                    if (placedSuccessfully) InventoryManager.Instance.UpdateItemPosition(droppedItem);
                }
            }
        }
        else
        {
            placedSuccessfully = GameManager.Instance.DropItemToField(droppedItem, this.inventory.OwnerState);
        }

        if (!placedSuccessfully)
        {
            this.UpdateUI();
            inventory.PlaceItem(droppedItem, droppedItem.GridX, droppedItem.GridY);
        }
        InventoryManager.Instance.StopDraggingItem();

        InventoryManager.Instance.CheckQuestOfferingStatus();
        if (_isQuestOfferingBox)
        {
            InventoryManager.Instance.CheckQuestOfferingStatus();
        }

        InventoryUI sourceUI = InventoryManager.Instance.SourceInventoryUI;
        if ((sourceUI != null && sourceUI._isQuestOfferingBox) || _isQuestOfferingBox)
        {
            InventoryManager.Instance.CheckQuestOfferingStatus();
        }

        InventoryManager.Instance.StopDraggingItem();
    }

    public void OnSlotPointerDown(int x, int y)
    {
        if (InventoryManager.Instance.CurrentDraggedItem != null || inventory == null) return;

        InventoryItem itemToDrag = inventory.RemoveItem(x, y);

        if (itemToDrag != null)
        {
            InventoryManager.Instance.SourceInventoryUI = this;
            InventoryManager.Instance.StartDraggingItem(itemToDrag);
            UpdateUI();

            if(_isQuestOfferingBox)
            {
                InventoryManager.Instance.CheckQuestOfferingStatus();
            }
        }
    }

    public void SetQuestOfferingMode(List<int> allowedIDs)
    {
        _isQuestOfferingBox = true;
        _allowedItemIDs = allowedIDs;
    }

    public void Initialize(Inventory inventoryData)
    {
        this.inventory = inventoryData;
        IsInitialized = true;
        UpdateUI();
    }

    public void UpdateUI()
    {
        foreach (var slotUI in slotUIList)
        {
            if (inventory != null)
            {
                InventoryItem item = inventory.GetItem(slotUI.GridX, slotUI.GridY);
                slotUI.UpdateItem(item);
            }
        }
    }

    private void CreateGridSlots()
    {
        foreach (Transform child in gridLayoutGroup) { Destroy(child.gameObject); }
        slotUIList.Clear();

        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                GameObject slotGO = Instantiate(slotPrefab, gridLayoutGroup);
                InventorySlotUI slotUI = slotGO.GetComponent<InventorySlotUI>();
                slotUI.SetSlot(x, y, this, slotSize);
                slotUIList.Add(slotUI);
                if (x >= usableWidth || y >= usableHeight) { slotUI.SetAvailability(false); }
            }
        }
    }

    public void UpdatePlacementPreview(InventorySlotUI targetSlot)
    {
        InventoryManager.Instance.ResetAllPreviews();
        if (targetSlot == null) return;

        InventoryItem draggedItem = InventoryManager.Instance.CurrentDraggedItem;
        if (draggedItem == null) return;

        if (targetSlot.inventoryUI != this)
        {
            targetSlot.inventoryUI.UpdatePlacementPreview(targetSlot);
            return;
        }

        bool canPlace = inventory.CanPlaceItem(draggedItem, targetSlot.GridX, targetSlot.GridY);
        Color previewColor = canPlace ? new Color(0, 1, 0, 0.5f) : new Color(1, 0, 0, 0.5f);

        for (int y = 0; y < draggedItem.height; y++)
        {
            for (int x = 0; x < draggedItem.width; x++)
            {
                if (draggedItem.occupiedCells[y, x])
                {
                    int checkX = targetSlot.GridX + x;
                    int checkY = targetSlot.GridY + y;
                    if (checkX < gridWidth && checkY < gridHeight)
                    {
                        slotUIList[checkY * gridWidth + checkX].SetPreviewColor(previewColor);
                    }
                }
            }
        }
    }

    private InventorySlotUI GetSlotUnderMouse()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        foreach (var result in results)
        {
            var slot = result.gameObject.GetComponent<InventorySlotUI>();
            if (slot != null) return slot;
        }
        return null;
    }

    public void ResetAllSlotPreviews()
    {
        foreach (var slot in slotUIList) { slot.ResetPreviewColor(); }
    }

    public void SetPulsating(bool enable)
    {
        isPulsating = enable;
        if (!enable && backgroundPanelImage != null)
        {
            var color = backgroundPanelImage.color;
            backgroundPanelImage.color = new Color(color.r, color.g, color.b, 1f);
        }
    }

    public Inventory GetInventory() { return inventory; }

    public void Open(InventoryAnimType animType = InventoryAnimType.FadeOnly)
    {
        gameObject.SetActive(true);
        canvasGroup.blocksRaycasts = false;

        switch (animType)
        {
            case InventoryAnimType.ScaleFade: 
                canvasGroup.alpha = 0f;
                transform.localScale = Vector3.zero * 0.5f;
                rectTransform.DOScale(originalScale, animationDuration).SetEase(Ease.OutBack);
                canvasGroup.DOFade(1f, animationDuration);
                break;
        }

        DOVirtual.DelayedCall(animationDuration, () => canvasGroup.blocksRaycasts = true);
    }
}