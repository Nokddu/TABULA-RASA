using System.Collections.Generic;

public enum EndActionTypes
{
    GainItem,
    RemoveItem,
    RequiredItem,
    RemoveGainItem,
    Select,
    Interaction,
    NextDialog,
    QuestStart,
    QuestComplete,
    QuestCancel,
    Puzzle,
    Ending,
    None
}

// 대화 이후 행동 노드 
public class EndActionNode 
{
    private string Id;
    private EndActionTypes EndActionType;
    private string Value1;
    private string Value2;
    private string Value3;

    private string[] ItemIdStrArray;
    private int[] ItemIdArray; // { 지울 아이템1, 얻을 아이템1, 지울 아이템2, 얻을 아이템2, ... }   (짝수 = 지울 아이템 (0, 2, 4, ...), 홀수 = 얻을 아이템 (1, 3, 5, ...))
    private string[] ItemCountStrArray;
    private int[] ItemCountArray; // { 지울 아이템1 개수, 얻을 아이템1 개수, 지울 아이템2 개수, 얻을 아이템2 개수, ... }
    private int RemoveItemCount;
    private List<int> RemoveItemCountList = new List<int>();
    private List<int> RemoveItemIdList = new List<int>(); // 지울 아이템 아이디 리스트 (짝수. i = 0, 2, 4, ...)
    private List<int> GainItemIdList = new List<int>(); // 얻을 아이템 아이디 리스트 (홀수. i = 1, 3, 5, ...)
    private List<int> GainItemCountList = new List<int>();

    private string[] SuccessFailStrArray;
    private int[] SuccessFailCancelArray;
    private int SuccessId;
    private int FailId;
    private int CancelId;
    
    private int TargetDialogId;
    private string UiText;

    private int QuestId;

    public EndActionNode(EndAction EndAction)
    {
        this.Id = EndAction.Id;
        this.EndActionType = (EndActionTypes)EndAction.EndActionType;
        this.Value1 = EndAction.Value1;
        this.Value2 = EndAction.Value2;
        this.Value3 = EndAction.Value3; 
    }

    public EndActionTypes ExcuteNode()
    {
        switch (EndActionType)
        {
            case EndActionTypes.GainItem:

                ItemIdStrArray = Value1.Split(",");
                ItemIdArray = new int[ItemIdStrArray.Length];
                for (int i = 0; i < ItemIdArray.Length; i++)
                {
                    ItemIdArray[i] = int.Parse(ItemIdStrArray[i]);
                }

                ItemCountStrArray = Value2.Split(",");
                ItemCountArray = new int[ItemCountStrArray.Length];
                for (int i = 0; i < ItemCountArray.Length; i++)
                {
                    ItemCountArray[i] = int.Parse(ItemCountStrArray[i]);
                }

                DialogManager.Instance.EndActionGainItem(ItemIdArray, ItemCountArray);

                break;
            case EndActionTypes.RemoveItem:

                ItemIdStrArray = Value1.Split(",");
                ItemIdArray = new int[ItemIdStrArray.Length];
                for (int i = 0; i < ItemIdArray.Length; i++)
                {
                    ItemIdArray[i] = int.Parse(ItemIdStrArray[i]);
                }

                ItemCountStrArray = Value2.Split(",");
                ItemCountArray = new int[ItemCountStrArray.Length];
                for (int i = 0; i < ItemCountArray.Length; i++)
                {
                    ItemCountArray[i] = int.Parse(ItemCountStrArray[i]);
                }

                SuccessFailStrArray = Value3.Split(",");
                SuccessFailCancelArray = new int[SuccessFailStrArray.Length];
                for (int i = 0; i < SuccessFailCancelArray.Length; i++)
                {
                    SuccessFailCancelArray[i] = int.Parse(SuccessFailStrArray[i]);
                }

                SuccessId = SuccessFailCancelArray[0];
                FailId = SuccessFailCancelArray[1];

                DialogManager.Instance.EndActionRemoveItem(DialogManager.Instance.CurrentState, ItemIdArray, ItemCountArray, SuccessId, FailId);

                break;

            case EndActionTypes.RequiredItem:

                ItemIdStrArray = Value1.Split(",");
                ItemIdArray = new int[ItemIdStrArray.Length];
                for (int i = 0; i < ItemIdArray.Length; i++)
                {
                    ItemIdArray[i] = int.Parse(ItemIdStrArray[i]);
                }

                ItemCountStrArray = Value2.Split(",");
                ItemCountArray = new int[ItemCountStrArray.Length];
                for (int i = 0; i < ItemCountArray.Length; i++)
                {
                    ItemCountArray[i] = int.Parse(ItemCountStrArray[i]);
                }
                SuccessFailStrArray = Value3.Split(",");
                SuccessFailCancelArray = new int[SuccessFailStrArray.Length];
                for (int i = 0; i < SuccessFailCancelArray.Length; i++)
                {
                    SuccessFailCancelArray[i] = int.Parse(SuccessFailStrArray[i]);
                }


                SuccessId = SuccessFailCancelArray[0];
                FailId = SuccessFailCancelArray[1];
                CancelId = SuccessFailCancelArray[2];

                DialogManager.Instance.EndActionRequiredItem(ItemIdArray, ItemCountArray, SuccessId, FailId, CancelId);

                break;

            case EndActionTypes.RemoveGainItem:

                ItemIdStrArray = Value1.Split(",");

                RemoveItemIdList.Clear();
                RemoveItemCountList.Clear();
                GainItemIdList.Clear();
                GainItemCountList.Clear();

                for (int i = 0; i < ItemIdStrArray.Length; i++)
                {
                    if(i % 2 == 0)
                    {
                        RemoveItemIdList.Add(int.Parse(ItemIdStrArray[i]));
                    }
                    else
                    {
                        GainItemIdList.Add(int.Parse(ItemIdStrArray[i]));
                    }
                }


                ItemCountStrArray = Value2.Split(",");

                for(int i = 0; i < ItemCountStrArray.Length; i++)
                {
                    if(i % 2 == 0)
                    {
                        RemoveItemCountList.Add(int.Parse(ItemCountStrArray[i]));
                    }
                    else
                    {
                        GainItemCountList.Add(int.Parse(ItemCountStrArray[i]));
                    }
                }

                SuccessFailStrArray = Value3.Split(",");
                SuccessFailCancelArray = new int[SuccessFailStrArray.Length];
                for (int i = 0; i < SuccessFailCancelArray.Length; i++)
                {
                    SuccessFailCancelArray[i] = int.Parse(SuccessFailStrArray[i]);
                }

                SuccessId = SuccessFailCancelArray[0];
                FailId = SuccessFailCancelArray[1];

                // (지울 아이템 아이디 리스트, 지울 아이템 개수 리스트, 얻을 아이템 아이디 리스트, 얻을 아이템 개수 리스트, 성공 아이디, 실패 아이디)
                // 플레이어(인간, 고양이, 개)의 인벤토리에서 RemoveGainItem
                DialogManager.Instance.EndActionRemoveGainItem(DialogManager.Instance.CurrentState, RemoveItemIdList, RemoveItemCountList, GainItemIdList, GainItemCountList, SuccessId, FailId);

                break;

            case EndActionTypes.Select:

                TargetDialogId = int.Parse(Value1);
                UiText = Value2;

                DialogManager.Instance.EndActionSelect(TargetDialogId, UiText);

                break;

            case EndActionTypes.Interaction:

                if (GameManager.Instance.Player.TargetNpc != null)
                {
                    if (GameManager.Instance.Player.TargetNpc.TryGetComponent<DoorOpen>(out var t))
                    {
                        if (!SaveManager.Instance.UserData.DoorInteracted.ContainsKey(GameManager.Instance.Player.TargetNpc.NpcData.Id))
                        {
                            SaveManager.Instance.UserData.DoorInteracted[GameManager.Instance.Player.TargetNpc.NpcData.Id] = true;
                        }
                    }
                }

                DialogManager.Instance.EndActionInteraction(Id);

                break;

            case EndActionTypes.NextDialog:

                TargetDialogId = int.Parse(Value1);

                DialogManager.Instance.EndActionNextDialog(TargetDialogId);

                break;

            case EndActionTypes.QuestStart:

                QuestId = int.Parse(Value1);

                DialogManager.Instance.EndActionQuestStart(QuestId, Id);

                break;

            case EndActionTypes.QuestComplete:

                ItemIdStrArray = Value1.Split(",");

                QuestId = int.Parse(ItemIdStrArray[0]);
                RemoveItemCount = int.Parse(Value2);

                // 삭제할 아이템이 있다면
                if(ItemIdStrArray.Length > 1)
                {
                    ItemIdArray = new int[ItemIdStrArray.Length - 1];
                    for(int i = 1; i < ItemIdStrArray.Length; i++)
                    {
                        ItemIdArray[i-1] = int.Parse(ItemIdStrArray[i]);
                    }
                }

                DialogManager.Instance.EndActionQuestComplete(QuestId, Id, ItemIdArray, RemoveItemCount);

                break;

            case EndActionTypes.QuestCancel:

                DialogManager.Instance.EndActionQuestCancel();

                break;

            case EndActionTypes.Puzzle:

                SuccessFailStrArray = Value3.Split(",");
                SuccessFailCancelArray = new int[SuccessFailStrArray.Length];
                for (int i = 0; i < SuccessFailCancelArray.Length; i++)
                {
                    SuccessFailCancelArray[i] = int.Parse(SuccessFailStrArray[i]);
                }

                SuccessId = SuccessFailCancelArray[0];
                FailId = SuccessFailCancelArray[1];

                DialogManager.Instance.EndActionInteraction(SuccessId, FailId);

                break;

            case EndActionTypes.Ending:

                DialogManager.Instance.EndActionEnding();

                break;
        }

        return EndActionType;
    }

    private void ValueSplit(EndActionTypes EndActionType)
    {
        switch (EndActionType)
        {
            case EndActionTypes.GainItem:
            case EndActionTypes.RemoveItem:
            case EndActionTypes.RequiredItem:
                ItemIdStrArray = Value1.Split(",");
                ItemIdArray = new int[ItemIdStrArray.Length];
                for (int i = 0; i < ItemIdArray.Length; i++)
                {
                    ItemIdArray[i] = int.Parse(ItemIdStrArray[i]);
                }

                ItemCountStrArray = Value2.Split(",");
                ItemCountArray = new int[ItemCountStrArray.Length];
                for (int i = 0; i < ItemCountArray.Length; i++)
                {
                    ItemCountArray[i] = int.Parse(ItemCountStrArray[i]);
                }

                if (EndActionType == EndActionTypes.RemoveItem || EndActionType == EndActionTypes.RequiredItem)
                {
                    SuccessFailStrArray = Value3.Split(",");
                    SuccessFailCancelArray = new int[SuccessFailStrArray.Length];
                    for (int i = 0; i < SuccessFailCancelArray.Length; i++)
                    {
                        SuccessFailCancelArray[i] = int.Parse(SuccessFailStrArray[i]);
                    }
                }
                break;

            case EndActionTypes.RemoveGainItem:
            case EndActionTypes.Puzzle:
                SuccessFailStrArray = Value3.Split(",");
                SuccessFailCancelArray = new int[SuccessFailStrArray.Length];
                for (int i = 0; i < SuccessFailCancelArray.Length; i++)
                {
                    SuccessFailCancelArray[i] = int.Parse(SuccessFailStrArray[i]);
                }
                break;
        }
    }
}
