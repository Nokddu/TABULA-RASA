using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using System;

public enum OnDialogEndAction
{
    GainItem,
    RemoveItem,
    RequiredItem,
    Select,
    Interaction,
    QuestStart,
    QuestComplete,
    QuestCancle
}
public class DialogManager : Singleton<DialogManager>
{
    private DataManager _dataManager;

    // -1 , 0 , 1 -> 매직넘버 변수화 리펙토링 어떨까.

    [SerializeField] private UIDialog _uiDialog;

    private int _id;
    private string _name;

    public Dictionary<string, EndActionNode> EndActionNodes { get; private set; }
    public Dictionary<int, DialogNode> DialogNodes { get; private set; }
    public Dictionary<int, EntityNode> EntityNodes { get; private set; }
    public Dictionary<int, List<DialogImage>> ImageNodes { get; private set; }
    public Dictionary<string, MileStoneNode> MileStoneNodes { get; private set; }

    private int _nowDialogTextIndex;

    private Transform _npcTransform;

    public bool IsTyping;

    private List<int> _selectButtonIdList;
    private List<string> _selectButtonTextList;

    private DialogNode _selectedDialogNode;
    private List<DialogLineNode> _currentDialogLineNodeList;

    public List<string> DialogLogList {  get; private set; }
    private string _speaker;

    private List<int> _currentLineNpcList;
    private List<int> _currentLineNpcPortraitList;

    public event Action ChangeField;

    public CurrentState CurrentState { get; private set; }

    private List<ItemInfo> RemoveItemList = new List<ItemInfo>();

    public bool FullTextCall { get; set; }

    protected override void Awake()
    {
        base.Awake();
    }
    private void Start()
    {
        _dataManager = DataManager.Instance;

        _selectButtonIdList = new List<int>();
        _selectButtonTextList = new List<string>();
        DialogLogList = new List<string>();

        EndActionNodes = _dataManager.EndActionNodes;
        DialogNodes = _dataManager.DialogNodes;
        EntityNodes = _dataManager.EntityNodes;
        ImageNodes = _dataManager.ImageNodes;
        MileStoneNodes = _dataManager.MileStoneNodes;

        _currentLineNpcList = new List<int>(2);
        _currentLineNpcPortraitList = new List<int>(2);
    }

    public void InitByEntity(int Id, GameObject npcTransform, CurrentState CurrentState, string Name)
    {
        _id = Id;
        _name = Name;
        _npcTransform = npcTransform.transform;
        this.CurrentState = CurrentState;

        // _id에 맞는 EntityNode에서 리프노드(DialogNode) 찾아서 실행 
        EntityNodes[_id].SelectLeafNode(CurrentState);
    }

    public void InitByEntity(int Id, GameObject npcTransform)
    { 
        EntityNodes[Id].SelectLeafNode();
        _npcTransform = npcTransform.transform;
    }

    private void ResetSelectList()
    {
        _selectButtonIdList.Clear();
        _selectButtonTextList.Clear();
    }

    public void PlayDialogNode(DialogNode TargetNode)
    {
        ResetSelectList();

        _selectedDialogNode = TargetNode;
        _currentDialogLineNodeList = TargetNode.DialogLineNodeList;
        _nowDialogTextIndex = 0;

        _uiDialog.ActivePanel();

        Show_NextDialog();
    }

    public void Show_NextDialog()
    {
        if (FullTextCall)
        {
            FullTextCall = false;
            return;
        }

        // 표시해야할 대사가 남은 경우, 대사가 다 끝났으면
        if(_currentDialogLineNodeList.Count > _nowDialogTextIndex)
        {
            // 타이핑 효과음
            SoundManager.Instance.Play_Sfx(SFX.Dialog);

            // 수락/거절 버튼 끄기 
            _uiDialog.InActiveButton();

            // 이전 진행중인 코루틴 멈추기 
            StopAllCoroutines();

            //IsTyping = true; // 한번 클릭하면 대사 다 나오게 하는 bool 변수 

            int Npc1 = _currentDialogLineNodeList[_nowDialogTextIndex].Npc1;
            int Npc1Portrait = _currentDialogLineNodeList[_nowDialogTextIndex].Npc1Portrait;
            int Npc2 = _currentDialogLineNodeList[_nowDialogTextIndex].Npc2;
            int Npc2Portrait = _currentDialogLineNodeList[_nowDialogTextIndex].Npc2Portrait;
            int NpcActive = _currentDialogLineNodeList[_nowDialogTextIndex].NpcActive;

            // 공통 정보는 컬렉션에 묶기. 
            _currentLineNpcList.Clear();
            _currentLineNpcPortraitList.Clear();
            _currentLineNpcList.Add(Npc1);
            _currentLineNpcList.Add(Npc2);
            _currentLineNpcPortraitList.Add(Npc1Portrait);
            _currentLineNpcPortraitList.Add(Npc2Portrait);

            // 초상화 세팅 
            _uiDialog.SetPortrait(_currentLineNpcList, _currentLineNpcPortraitList, NpcActive);

            // 대화 배경화면 세팅
            _uiDialog.SetBackground(_currentDialogLineNodeList[_nowDialogTextIndex].Background);

            // 대화 속도 설정 
            _uiDialog.SetTypingSpeed(_currentDialogLineNodeList[_nowDialogTextIndex].TypingSpeed);

            // 화자 설정
            _uiDialog.SetSpeaker(_dataManager.NpcNodes[_currentDialogLineNodeList[_nowDialogTextIndex].Speaker].Name);

            // 대사 출력 
            StartCoroutine(_uiDialog.SetDialogTextGradually(_currentDialogLineNodeList[_nowDialogTextIndex].Scripts));

            // 대사 로그 저장. 
            DialogLogList.Add($"{_dataManager.NpcNodes[_currentDialogLineNodeList[_nowDialogTextIndex].Speaker].Name}: {_currentDialogLineNodeList[_nowDialogTextIndex].Scripts}");

            // 다음 대사를 위해 인덱스를 ++
            _nowDialogTextIndex++;

            return;
        }

        // 대화가 끝나고나서 해야할 일이 없으면 
        if (_selectedDialogNode.EndActionCount == 0)
        {
            // 패널 비활성화 (대화창 끄기)
            _uiDialog.InActivePanel();

            // 배경화면 끄기 (배경화면이 활성화되어 있다면)
            _uiDialog.InActiveBackground();

            return;
        }
        // 표시해야할 대사가 없고, 대화가 끝나고나서 해야할 일이 있으면 
        else
        {
            // EndAction 실행 
            _selectedDialogNode.SelectLeafNode();
        }
    }

    public void EndActionGainItem(int[] ItemIdArray, int[] ItemCountArray)
    {
        // ItemIdArray[i]을 ItemCountArray[i]개 드롭
        for(int i = 0; i < ItemIdArray.Length; i++)
        {
            for(int j = 0; j < ItemCountArray[i]; j++)
            {
                AreaManager.GenerateGroundItem(ItemIdArray[i], new Vector2(_npcTransform.position.x, _npcTransform.position.y));
            }
        }

        SaveManager.Instance.UserData.NpcRewards[_id] = true;
        _uiDialog.InActivePanel();
    }

    public void EndActionRemoveItem(CurrentState CurrentState, int[] RemoveItemIdArray, int[] RemoveItemCountArray, int SuccessId, int FailId)
    {
        bool IsRemoveItem = false;

        // 지울 아이템이 인벤토리에 있는가? 
        for (int i = 0; i < RemoveItemIdArray.Length; i++)
        {
            switch (CurrentState)
            {
                case CurrentState.Human:

                    foreach (ItemInfo ItemInfo in SaveManager.Instance.UserData.PlayerItemData.HumanInventory)
                    {
                        // 인벤토리에 RemoveItemIdArray[i]가 있다면
                        if (RemoveItemIdArray[i] == ItemInfo.Id)
                        {
                            RemoveItemList.Add(ItemInfo);
                        }
                    }

                    // RemoveItemIdList[i]가 RemoveItemCountList[i]개 있다면 
                    if (RemoveItemList.Count == RemoveItemCountArray[i])
                    {
                        foreach (ItemInfo item in RemoveItemList)
                        {
                            SaveManager.Instance.UserData.PlayerItemData.HumanInventory.Remove(item);
                        }

                        // 아이템 제거 완료 
                        IsRemoveItem = true;
                    }

                    break;

                case CurrentState.Cat:

                    foreach (ItemInfo ItemInfo in SaveManager.Instance.UserData.PlayerItemData.CatInventory)
                    {
                        if (RemoveItemIdArray[i] == ItemInfo.Id)
                        {
                            RemoveItemList.Add(ItemInfo);
                        }
                    }

                    if (RemoveItemList.Count == RemoveItemCountArray[i])
                    {
                        foreach (ItemInfo item in RemoveItemList)
                        {
                            SaveManager.Instance.UserData.PlayerItemData.CatInventory.Remove(item);
                        }

                        IsRemoveItem = true;
                    }

                    break;

                case CurrentState.Dog:

                    foreach (ItemInfo ItemInfo in SaveManager.Instance.UserData.PlayerItemData.DogInventory)
                    {
                        if (RemoveItemIdArray[i] == ItemInfo.Id)
                        {
                            RemoveItemList.Add(ItemInfo);
                        }
                    }

                    if (RemoveItemList.Count == RemoveItemCountArray[i])
                    {
                        foreach (ItemInfo item in RemoveItemList)
                        {
                            SaveManager.Instance.UserData.PlayerItemData.DogInventory.Remove(item);
                        }

                        IsRemoveItem = true;
                    }

                    break;
            }
        }

        if (IsRemoveItem)
        {
            RemoveItemList.Clear();
            PlayDialogNode(DialogNodes[SuccessId]);
        }
        else
        {
            RemoveItemList.Clear();
            PlayDialogNode(DialogNodes[FailId]);
        }
    }

    public void EndActionRequiredItem(int[] ItemIdArray, int[] ItemCountArray, int SuccessId, int FailId, int CancelId)
    {
        List<QuestItemRequirement> requirements = new List<QuestItemRequirement>();

        for (int i = 0; i < ItemIdArray.Length; i++)
        {
            foreach (int id in ItemIdArray)
            {
                Debug.Log($"[DialogManager] 시작 시점의 요구 아이템 ID: {id}");
            }
            requirements.Add(new QuestItemRequirement { itemID = ItemIdArray[i], quantity = ItemCountArray[i] });
        }

        _uiDialog.InActivePanel();

        InventoryManager.Instance.BeginQuestSubmission(requirements, SuccessId, FailId, CancelId);
    }


    public void EndActionRemoveGainItem(CurrentState CurrentState, List<int> RemoveItemIdList, List<int> RemoveItemCountList, List<int> GainItemIdList, List<int> GainItemCountList, int SuccessId, int FailId)
    {
        bool IsRemoveItem = false;

        // 지울 아이템이 인벤토리에 있는가? 
        for (int i = 0; i < RemoveItemIdList.Count; i++)
        {
            switch(CurrentState)
            {
                case CurrentState.Human:

                    foreach (ItemInfo ItemInfo in SaveManager.Instance.UserData.PlayerItemData.HumanInventory)
                    {
                        // 인벤토리에 RemoveItemIdList[i]가 있다면
                        if (RemoveItemIdList[i] == ItemInfo.Id)
                        {
                            RemoveItemList.Add(ItemInfo);
                        }
                    }

                    // RemoveItemIdList[i]가 RemoveItemCountList[i]개 있다면 
                    if(RemoveItemList.Count == RemoveItemCountList[i])
                    {
                        foreach(ItemInfo item in RemoveItemList)
                        {
                            SaveManager.Instance.UserData.PlayerItemData.HumanInventory.Remove(item);
                        }

                        // 아이템 제거 완료 
                        IsRemoveItem = true;
                        RemoveItemList.Clear();
                    }

                    break;

                case CurrentState.Cat:

                    foreach (ItemInfo ItemInfo in SaveManager.Instance.UserData.PlayerItemData.CatInventory)
                    {
                        if (RemoveItemIdList[i] == ItemInfo.Id)
                        {
                            RemoveItemList.Add(ItemInfo);
                        }
                    }

                    if (RemoveItemList.Count == RemoveItemCountList[i])
                    {
                        foreach (ItemInfo item in RemoveItemList)
                        {
                            SaveManager.Instance.UserData.PlayerItemData.CatInventory.Remove(item);
                        }

                        IsRemoveItem = true;
                        RemoveItemList.Clear();
                    }

                    break;

                case CurrentState.Dog:

                    foreach (ItemInfo ItemInfo in SaveManager.Instance.UserData.PlayerItemData.DogInventory)
                    {
                        if (RemoveItemIdList[i] == ItemInfo.Id)
                        {
                            RemoveItemList.Add(ItemInfo);
                        }
                    }

                    if (RemoveItemList.Count == RemoveItemCountList[i])
                    {
                        foreach (ItemInfo item in RemoveItemList)
                        {
                            SaveManager.Instance.UserData.PlayerItemData.DogInventory.Remove(item);
                        }

                        IsRemoveItem = true;
                        RemoveItemList.Clear();
                    }

                    break;
            }
        }

        if (IsRemoveItem)
        {
            RemoveItemList.Clear();

            for (int i = 0; i < GainItemIdList.Count; i++) 
            {
                for(int j = 0; j < GainItemCountList[i]; j++)
                {
                    AreaManager.GenerateGroundItem(GainItemIdList[i], new Vector2(_npcTransform.position.x, _npcTransform.position.y));
                }
            }

            SaveManager.Instance.UserData.NpcRewards[_id] = true;

            PlayDialogNode(DialogNodes[SuccessId]);
        }
        else
        {
            RemoveItemList.Clear();
            PlayDialogNode(DialogNodes[FailId]);
        }
    }

    public void EndActionSelect(int TargetDialogId, string UiText)
    {
        // 선택지 버튼 텍스트, 선택지 버튼에 연결할 아이디 세팅
        _selectButtonIdList.Add(TargetDialogId);
        _selectButtonTextList.Add(UiText);
    }

    public void EndActionInteraction(string Id)
    {
        IInteraction interactable = _npcTransform.GetComponent<IInteraction>();
        if (interactable != null)
        {
            interactable.Interact();

            if (MileStoneNodes.ContainsKey(Id))
            {
                UIManager.Instance.CurrentTxt.text = MileStoneNodes[Id].MileStone;
            }
        }
        _uiDialog.InActivePanel();
    }

    public void EndActionNextDialog(int TargetDialogId)
    {
        PlayDialogNode(DialogNodes[TargetDialogId]);
    }

    public void EndActionInteraction(int SuccessId, int FailId)
    {
        IPuzzle puzzle = _npcTransform.GetComponent<IPuzzle>();
        if(puzzle != null)
        {
            puzzle.Interact(SuccessId, FailId);
        }
    }

    public void EndActionQuestStart(int QuestId, string Id)
    {
        if (!SaveManager.Instance.UserData.QuestInfo.ContainsKey(QuestId))
        {
            SaveManager.Instance.UserData.QuestInfo[QuestId] = Condition.QuestInProgress;

            if (MileStoneNodes.ContainsKey(Id))
            {
                UIManager.Instance.CurrentTxt.text = MileStoneNodes[Id].MileStone;
            }
        }

        _uiDialog.InActivePanel();
    }

    public void EndActionQuestComplete(int QuestId, string Id, int[] RemoveItemIdArray, int RemoveItemCount)
    {
        // 제출 아이템 제거
        for (int i = 0; i < RemoveItemIdArray.Length; i++)
        {
            foreach (ItemInfo ItemInfo in SaveManager.Instance.UserData.PlayerItemData.HumanInventory)
            {
                if (RemoveItemIdArray[i] == ItemInfo.Id)
                {
                    RemoveItemList.Add(ItemInfo);
                }
            }

            if (RemoveItemList.Count == RemoveItemCount)
            {
                foreach (ItemInfo item in RemoveItemList)
                {
                    SaveManager.Instance.UserData.PlayerItemData.HumanInventory.Remove(item);
                }
            }
        }
        RemoveItemList.Clear();

        // 퀘스트 완료 처리
        var _save = SaveManager.Instance;
        _save.UserData.QuestInfo[QuestId] = Condition.QuestCompleted;
        InventoryManager.Instance.OfferingInventoryDestroy();

        if (MileStoneNodes.ContainsKey(Id))
        {
            UIManager.Instance.CurrentTxt.text = MileStoneNodes[Id].MileStone;
        }

        _save.UserData.Level += 1;
        GameManager.Instance.Player.LevelUI.Invoke();

        _save.UserData.PlayerPosX = GameManager.Instance.Player.RayPoint.position.x;
        _save.UserData.PlayerPosY = GameManager.Instance.Player.RayPoint.position.y;

        GameManager.Instance.PlayerPos.x = _save.UserData.PlayerPosX;
        GameManager.Instance.PlayerPos.y = _save.UserData.PlayerPosY;

        _save.Save_Data(_save.UserData, _save.SystemData);
        _save.Load_Data();
        
        InventoryManager.Instance.HideAllInventories();

        _uiDialog.InActivePanel();

        ChangeField?.Invoke();
    }
    public void EndActionQuestCancel()
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.ReturnQuestItemsAndClose();
        }
        else
        {

        }
        InventoryManager.Instance.OfferingInventoryDestroy();
        InventoryManager.Instance.HideAllInventories();

        _uiDialog.InActivePanel();
    }

    public void EndActionEnding()
    {
        SceneLoadManager.Instance.LoadScene(SceneType.Title);

        SoundManager.Instance.SilenceMixer(-20f);

        _uiDialog.InActivePanel();
    }

    // EndAction 실행 후 후처리 실행. 
    public void EndActionExcuteComplete(EndActionTypes ExcutedEndActionType)
    {
        switch (ExcutedEndActionType)
        {
            case EndActionTypes.GainItem:
                SoundManager.Instance.Play_Sfx(SFX.DropItem);
                break;
            case EndActionTypes.RemoveItem:
                break;
            case EndActionTypes.RequiredItem:
                break;
            case EndActionTypes.RemoveGainItem:
                break;
            case EndActionTypes.Select:
                _uiDialog.ActiveButton(); 
                _uiDialog.SetButton2(_selectButtonIdList, _selectButtonTextList);
                break;
            case EndActionTypes.Interaction:
                break;
            case EndActionTypes.NextDialog:
                break;
            case EndActionTypes.QuestStart:
                break;
            case EndActionTypes.QuestComplete:
                break;
            case EndActionTypes.QuestCancel:
                break;
            case EndActionTypes.Puzzle:
                break;
        }
    }
}