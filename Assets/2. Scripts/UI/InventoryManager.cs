using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : Singleton<InventoryManager>
{
    private Dictionary<int, Inventory> _inventoryData = new Dictionary<int, Inventory>();
    private Dictionary<int, InventoryUI> _inventoryUI = new Dictionary<int, InventoryUI>();

    [Header("인벤토리 프리팹")]
    public GameObject humanInventoryPrefab;
    public GameObject catInventoryPrefab;
    public GameObject dogInventoryPrefab;
    public GameObject npc1_InventoryPrefab;

    [Header("퀘스트 인벤토리 프리팹")]
    public GameObject offeringInventoryPrefab;

    [Header("UI 위치값")]
    [SerializeField] private Vector2 _playerInventoryPosition = new Vector2(1000, -538);
    [SerializeField] private Vector2 _secondaryInventoryPosition = new Vector2(1600, -538);

    private InventoryUI _questOfferingUI;
    private List<QuestItemRequirement> _currentQuestRequirements;
    private int _questSuccessDialogId;
    private int _questFailDialogId;
    private int _questCancelDialogId;

    [Header("UI가 생성될 부모 캔버스")]
    public Transform uiCanvas;

    [Header("전역 UI")]
    public Image globalDraggedItemIcon;
    private RectTransform globalDraggedItemRect;

    private UIDialog _questDialogController;

    public bool IsInventory { get; private set; }
    private List<InventoryUI> _currentOpenInventories = new List<InventoryUI>();

    public InventoryItem CurrentDraggedItem { get; private set; }
    public InventoryUI SourceInventoryUI { get; set; }

    protected override void Awake()
    {
        base.Awake();
        InitializeInventories();

        if (globalDraggedItemIcon != null)
        {
            globalDraggedItemRect = globalDraggedItemIcon.GetComponent<RectTransform>();
            globalDraggedItemIcon.gameObject.SetActive(false);
            globalDraggedItemIcon.raycastTarget = false;
        }
    }

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => SaveManager.Instance.IsLoadCompleted);

        LoadAllInventoriesFromSaveData();
    }

    private void Update()
    {
        if (CurrentDraggedItem != null)
        {
            globalDraggedItemRect.position = Input.mousePosition;

            if (Input.GetKeyDown(KeyCode.R))
            {
                CurrentDraggedItem.Rotate();
                globalDraggedItemRect.sizeDelta = new Vector2(CurrentDraggedItem.width * 100, CurrentDraggedItem.height * 100);
            }
        }
    }
    public void StartDraggingItem(InventoryItem item)
    {
        if (item == null) return;
        CurrentDraggedItem = item;

        globalDraggedItemIcon.sprite = Resources.Load<Sprite>(item.ItemData.ImagePath);
        //Resources 로드가 지속적부터 되고있음. ItemData 스크립트에서 개선하면 괜찮을듯함.
        globalDraggedItemRect.sizeDelta = new Vector2(item.width * 100, item.height * 100);
        globalDraggedItemIcon.gameObject.SetActive(true);
    }
    public void StopDraggingItem()
    {
        CurrentDraggedItem = null;
        SourceInventoryUI = null;
        globalDraggedItemIcon.gameObject.SetActive(false);
        ResetAllPreviews();
        UpdateAllOpenInventories();
    }

    private void InitializeInventories()
    {
        //프리팹을 List로 변경 예정하고 리스트를 순회하여 Inventory와 UI등록 시키면 어떨까.
        RegisterInventory((int)CurrentState.Human, 4, 4, 4, 4);
        RegisterInventory((int)CurrentState.Cat, 2, 1, 2, 1);
        RegisterInventory((int)CurrentState.Dog, 3, 2, 3, 2);

        if (humanInventoryPrefab != null)
            _inventoryUI[(int)CurrentState.Human] = CreateInventoryUI(humanInventoryPrefab);
        if (catInventoryPrefab != null)
            _inventoryUI[(int)CurrentState.Cat] = CreateInventoryUI(catInventoryPrefab);
        if (dogInventoryPrefab != null)
            _inventoryUI[(int)CurrentState.Dog] = CreateInventoryUI(dogInventoryPrefab);

        foreach (var ui in _inventoryUI.Values)
        {
            if (ui != null) ui.gameObject.SetActive(false);
        }
    }

    private InventoryUI CreateInventoryUI(GameObject prefab)
    {
        GameObject uiObject = Instantiate(prefab, uiCanvas);
        return uiObject.GetComponent<InventoryUI>();
    }

    public void BeginQuestSubmission(List<QuestItemRequirement> requirements, int successId, int failId, int cancelId)
    {
        if (IsInventory) return;

        _currentQuestRequirements = requirements;
        _questSuccessDialogId = successId;
        _questFailDialogId = failId;
        _questCancelDialogId = cancelId;

        GameObject offeringObject = Instantiate(offeringInventoryPrefab, uiCanvas);
        _questOfferingUI = offeringObject.GetComponent<InventoryUI>();

        _questDialogController = offeringObject.GetComponent<UIDialog>();
        if (_questDialogController == null)
        {
            return;
        }

        Inventory offeringInventory = new Inventory(
            _questOfferingUI.gridWidth, _questOfferingUI.gridHeight,
            _questOfferingUI.usableWidth, _questOfferingUI.usableHeight,
            (CurrentState)(-1)
        );
        _questOfferingUI.Initialize(offeringInventory);

        List<int> allowedItemIDs = requirements.Select(req => req.itemID).ToList();
        _questOfferingUI.SetQuestOfferingMode(allowedItemIDs);
        _questDialogController.SetCancelButton(_questCancelDialogId);

        _questDialogController.SetCancelButton(cancelId);

        OpenQuestInventories(_questOfferingUI);

        CheckQuestOfferingStatus();
    }
    
    public void CheckQuestOfferingStatus()
    {
        if (_questOfferingUI == null || _questDialogController == null) return;

        bool canSucceed = _questOfferingUI.GetInventory().CheckRequirements(_currentQuestRequirements);

        if (canSucceed)
        {
            _questDialogController.SetAcceptButtonState(true, _questSuccessDialogId, _questFailDialogId);
        }
        else
        {
            _questDialogController.SetAcceptButtonState(false, _questSuccessDialogId, _questFailDialogId);
        }
    }


    public void EndQuestSubmission()
    {
        HideAllInventories();

        OfferingInventoryDestroy();

        _questOfferingUI = null;
        _currentQuestRequirements = null;
    }
    public void ReturnQuestItemsAndClose()
    {
        if (_questOfferingUI == null)
        {
            HideAllInventories();
            return;
        }

        Inventory offeringInventory = _questOfferingUI.GetInventory();
        Inventory playerInventory = GetInventoryData((int)CurrentState.Human); 
        PlayerItemData playerItemData = SaveManager.Instance.UserData.PlayerItemData;
        List<ItemInfo> playerSaveDataList = playerItemData.GetInventoryList(CurrentState.Human); 

        if (offeringInventory == null || playerInventory == null || playerItemData == null || playerSaveDataList == null)
        {
            HideAllInventories(); 
            return;
        }

        List<InventoryItem> itemsToReturn = offeringInventory.GetAllItems();
        foreach (InventoryItem item in itemsToReturn)
        {
            bool addedToPlayerInv = playerInventory.AddItemToFirstAvailableSlot(item);

            if (addedToPlayerInv)
            {
                UpdateItemPosition(item);
                if (!playerSaveDataList.Contains(item.ItemInfo)) 
                {
                    playerSaveDataList.Add(item.ItemInfo);
                }
                item.ItemInfo.IsInInventory = true;

                playerItemData.ItemGet(item.ItemInfo, CurrentState.Human);
            }
            else
            {
                NotificationManager.Instance.ShowInventoryFullMessage(); 
                                                                        
                if (GameManager.Instance != null && GameManager.Instance.Player != null)
                {
                    GameManager.Instance.DropItemToField(item, (CurrentState)(-1)); 
                }
            }
        }

        HideAllInventories();
    }

    public void ShowTradeInventories(CurrentState playerState, CurrentState targetState)
    {
        if (IsInventory) { HideAllInventories(); return; }

        InventoryUI playerUI = GetInventoryUI((int)playerState);
        InventoryUI targetUI = GetInventoryUI((int)targetState);

        if (playerUI != null && targetUI != null && playerUI != targetUI)
        {
            playerUI.gameObject.SetActive(true);
            targetUI.gameObject.SetActive(true);

            if (globalDraggedItemRect != null)
            {
                globalDraggedItemRect.SetAsLastSibling();
            }

            playerUI.GetComponent<RectTransform>().anchoredPosition = _playerInventoryPosition;
            targetUI.GetComponent<RectTransform>().anchoredPosition = _secondaryInventoryPosition;

            playerUI.SetPulsating(false);
            targetUI.SetPulsating(true);

            if (!playerUI.IsInitialized) playerUI.Initialize(GetInventoryData((int)playerState));
            if (!targetUI.IsInitialized) targetUI.Initialize(GetInventoryData((int)targetState));

            playerUI.UpdateUI();
            targetUI.UpdateUI();

            _currentOpenInventories.Add(playerUI);
            _currentOpenInventories.Add(targetUI);

            GameManager.Instance.Player.InteractOn(Vector2.down,true,false,false);
            IsInventory = true;
        }
    }
    public bool TransferItem(InventoryItem item, Inventory fromInventory, Inventory toInventory, int targetX, int targetY)
    {
        bool added = toInventory.PlaceItem(item, targetX, targetY);

        if (added)
        {
            SaveManager.Instance.UserData.PlayerItemData.TransferItem(item.ItemInfo, fromInventory.OwnerState, toInventory.OwnerState);
            UpdateItemPosition(item);
            return true;
        }
        else
        {
            return false;
        }
    }
    public void UpdateAllOpenInventories()
    {
        foreach (var ui in _currentOpenInventories.Where(ui => ui != null && ui.gameObject.activeInHierarchy))
        {
            ui.UpdateUI();
        }
    }
    public InventoryUI GetTargetInventoryAtMousePosition(InventoryUI askingUI)
    {
        return _currentOpenInventories.FirstOrDefault
            (ui => ui != null && ui != askingUI && ui.IsMouseOverUI);
    }

    public void ShowInventory(CurrentState state)
    {
        LoadAllInventoriesFromSaveData();
        if (IsInventory) { HideAllInventories(); return; }

        InventoryUI targetInvUI = GetInventoryUI((int)state);

        if (targetInvUI != null)
        {
            InventoryAnimType animType;

            switch (state)
            {
                case CurrentState.Human: animType = InventoryAnimType.ScaleFade; break;
                case CurrentState.Cat: animType = InventoryAnimType.ScaleFade; break;
                case CurrentState.Dog: animType = InventoryAnimType.ScaleFade; break;
                default: animType = InventoryAnimType.ScaleFade; break;
            }
            targetInvUI.Open(animType);

            targetInvUI.gameObject.SetActive(true);
            targetInvUI.GetComponent<RectTransform>().anchoredPosition = _playerInventoryPosition;

            if (globalDraggedItemRect != null)
            {
                globalDraggedItemRect.SetAsLastSibling();
            }
            Inventory targetInvData = GetInventoryData((int)state);
            if (!targetInvUI.IsInitialized)
            {
                targetInvUI.Initialize(targetInvData);
            }
            targetInvUI.UpdateUI();

            _currentOpenInventories.Add(targetInvUI);
        }

        IsInventory = true;
        GameManager.Instance.Player.InteractOn(Vector2.down,true,false,false);
    }

    public void HideAllInventories()
    {
        foreach (var ui in _inventoryUI.Values)
        {
            if (ui != null)
            {
                ui.SetPulsating(false);
                ui.gameObject.SetActive(false);
            }
        }
        //---------------------------------------
        if (TooltipManager.Instance != null)
        {
            TooltipManager.Instance.HideTooltip();
        }

        _currentOpenInventories.Clear();
        IsInventory = false;
        GameManager.Instance.Player.InteractOff();

        if (_questOfferingUI != null)
        {
            Destroy(_questOfferingUI.gameObject);
            _questOfferingUI = null;
        }

        if (TooltipManager.Instance != null)
        {
            TooltipManager.Instance.HideTooltip();
        }
        _currentOpenInventories.Clear();
        IsInventory = false;
        if (GameManager.Instance != null && GameManager.Instance.Player != null)
            GameManager.Instance.Player.InteractOff();
        //-> 변경후
        //if (TooltipManager.Instance != null)
        //{
        //    TooltipManager.Instance.HideTooltip();
        //}

        //_currentOpenInventories.Clear();
        //IsInventory = false;

        //if (GameManager.Instance != null && GameManager.Instance.Player != null)
        //    GameManager.Instance.Player.InteractOff();

        //if (_questOfferingUI != null)
        //{
        //    Destroy(_questOfferingUI.gameObject);
        //    _questOfferingUI = null;
        //}
    }
    public void OfferingInventoryDestroy()
    {
        if (_questOfferingUI != null)
        {
            Destroy(_questOfferingUI.gameObject);
            _questOfferingUI = null;
        }
    }

    public bool PickUpItem(FieldItem fieldItem, int currentState)
    {
        if ((CurrentState)currentState == CurrentState.Ghost)
        {
            NotificationManager.Instance.ShowGhostMessage();

            return false;
        }

        LoadAllInventoriesFromSaveData();

        Inventory targetInventory = GetInventoryData(currentState);
        if (targetInventory == null) return false;

        ItemData itemData = DataManager.Instance.ItemDB.Get(fieldItem.TargetItemInfo.Id);
        if (itemData == null) return false;

        InventoryItem newItem = new InventoryItem(fieldItem.TargetItemInfo, itemData);
        bool added = targetInventory.AddItemToFirstAvailableSlot(newItem);

        if (added)
        {
            SaveManager.Instance.UserData.PlayerItemData.ItemGet(newItem.ItemInfo, (CurrentState)currentState);
            UpdateItemPosition(newItem);

            InventoryUI targetUI = GetInventoryUI(currentState);
            if (targetUI != null && targetUI.gameObject.activeInHierarchy)
            {
                targetUI.UpdateUI();
            }
            Destroy(fieldItem.gameObject);
            return true;
        }
        else
        {
            NotificationManager.Instance.ShowInventoryFullMessage();
            return false;
        }
    }

    public void UpdateItemPosition(InventoryItem item)
    {
        if (item == null || item.ItemInfo == null) return;
        item.ItemInfo.GridX = item.GridX;
        item.ItemInfo.GridY = item.GridY;
        item.ItemInfo.Rotation = item.rotationAngle;
    }

    private void LoadAllInventoriesFromSaveData()
    {
        foreach (var state in _inventoryData.Keys)
        {
            LoadInventoryData(_inventoryData[state], (CurrentState)state);
        }
    }

    private void LoadInventoryData(Inventory inventory, CurrentState state)
    {
        if (inventory == null) return;
        inventory.Clear();

        var playerItemData = SaveManager.Instance.UserData.PlayerItemData;
        var itemList = playerItemData.GetInventoryList(state);
        if (itemList == null) return;

        foreach (var itemInfo in itemList)
        {
            ItemData itemData = DataManager.Instance.ItemDB.Get(itemInfo.Id);
            if (itemData != null)
            {
                InventoryItem newItem = new InventoryItem(itemInfo, itemData);
                inventory.PlaceItem(newItem, itemInfo.GridX, itemInfo.GridY);
            }
        }
    }

    public void OpenQuestInventories(InventoryUI offeringUI)
    {
        if (IsInventory) { HideAllInventories(); return; }

       InventoryUI playerUI = GetInventoryUI((int)CurrentState.Human);

        if (playerUI != null && offeringUI != null)
        {
            playerUI.gameObject.SetActive(true);
            offeringUI.gameObject.SetActive(true);

            if (globalDraggedItemRect != null)
            {
                globalDraggedItemRect.SetAsLastSibling();
            }

            playerUI.GetComponent<RectTransform>().anchoredPosition = _playerInventoryPosition;
            offeringUI.GetComponent<RectTransform>().anchoredPosition = _secondaryInventoryPosition;

            offeringUI.SetPulsating(true);

            if (!playerUI.IsInitialized) playerUI.Initialize(GetInventoryData((int)CurrentState.Human));

            playerUI.UpdateUI();
            offeringUI.UpdateUI();

            _currentOpenInventories.Add(playerUI);
            _currentOpenInventories.Add(offeringUI);

            GameManager.Instance.Player.InteractOn(Vector2.down, true, false, false);
            IsInventory = true;
        }
    }

    public Inventory RegisterInventory(int ownerId, int gridW, int gridH, int usableW, int usableH)
    {
        if (_inventoryData.ContainsKey(ownerId))
        {
            return _inventoryData[ownerId];
        }
        else
        {
            Inventory newInventory = new Inventory(gridW, gridH, usableW, usableH, (CurrentState)ownerId);
            _inventoryData[ownerId] = newInventory;
            return newInventory;
        }
    }

    public void ShowTradeWithNPC(CurrentState playerState, int npcId)
    {
        if (IsInventory) { HideAllInventories(); return; }

        InventoryUI playerUI = GetInventoryUI((int)playerState);
        Inventory npcData = GetInventoryData(npcId);

        if (npcData == null)
        {
            return;
        }

        GameObject npcUIObject = Instantiate(npc1_InventoryPrefab, uiCanvas);
        InventoryUI npcUI = npcUIObject.GetComponent<InventoryUI>();
        npcUI.Initialize(npcData); 
        npcUIObject.name = $"NPC_{npcId}_Inventory(Clone)";

        if (playerUI != null && npcUI != null)
        {
            playerUI.gameObject.SetActive(true);

            if (globalDraggedItemRect != null)
            {
                globalDraggedItemRect.SetAsLastSibling();
            }

            playerUI.GetComponent<RectTransform>().anchoredPosition = _playerInventoryPosition;
            npcUI.GetComponent<RectTransform>().anchoredPosition = _secondaryInventoryPosition;

            playerUI.UpdateUI();
            npcUI.UpdateUI();

            _currentOpenInventories.Add(playerUI);
            _currentOpenInventories.Add(npcUI);

            GameManager.Instance.Player.InteractOn(Vector2.down, true, false, false);
            IsInventory = true;
        }
    }

    public Inventory GetInventoryData(int state)
    {
        _inventoryData.TryGetValue(state, out Inventory data);
        return data;
    }

    private InventoryUI GetInventoryUI(int state)
    {
        _inventoryUI.TryGetValue(state, out InventoryUI ui);
        return ui;
    }

    public void ResetAllPreviews()
    {
        foreach (var ui in _currentOpenInventories)
        {
            if (ui != null && ui.gameObject.activeInHierarchy)
            {
                ui.ResetAllSlotPreviews();
            }
        }
    }
}