using System.Collections.Generic;
using UnityEngine;

public enum Condition
{
    RequiredItem,
    NoRequiredItem,
    RewardDone,
    Card,
    QuestNotStarted,
    QuestInProgress,
    QuestCompleted,

    Always = 9999
}

public class NormalEntityDialogNode : IEntityDialogNode
{

    private CurrentState _state;
    private Condition TargetCondition;
    private string[] TargetConditionIdStrArray;
    private int[] TargetConditionIdArray;
    public DialogNode TargetDialogNode;
    private DialogNode _randomDialogNode = new DialogNode();

    private int _npcId;

    private int BaseValue;

    private int Start;
    private int End;

    private int RandomTargetDialogId;

    public NormalEntityDialogNode(Dialog Dialog)
    {
        _npcId = Dialog.Id;
        _state = Dialog.State;

        // 파싱해서 id, condition 초기화
        string[] strs = Dialog.Condition.Split("$");

        // 조건이 있는 경우 변수에 초기화
        if(strs.Length > 0 && !strs[0].Equals(""))
        {
            TargetCondition = (Condition)int.Parse(strs[0]);

            if (strs.Length > 1)
            {
                TargetConditionIdStrArray = strs[1].Split(",");
                TargetConditionIdArray = new int[TargetConditionIdStrArray.Length];
                
                for(int i = 0; i < TargetConditionIdArray.Length; i++)
                {
                    TargetConditionIdArray[i] = int.Parse(TargetConditionIdStrArray[i]);
                }
            }
        }
        else
        {
            TargetCondition = Condition.Always;
        }

        // 리프 노드 
        this.TargetDialogNode = DataManager.Instance.DialogNodes[Dialog.TargetDialogId];
    }

    // 실행할 가능한 노드 탐색
    public bool TrySelectNode(CurrentState CurrentState)
    {
        bool ret1 = false;
        bool ret2 = false;

        if(this._state == CurrentState)
        {
            ret1 = true;
        }

        switch (TargetCondition)
        {
            case Condition.RequiredItem:

                // 빙의체 인벤토리에서 요구 아이템이 있는지 확인 
                if (CurrentState == CurrentState.Human)
                {
                    for(int i = 0; i < TargetConditionIdArray.Length; i++)
                    {
                        // TargetConditionIdArray[i]가 인간 인벤토리에 있는가? 
                        foreach (ItemInfo HumanInventoryItemInfo in SaveManager.Instance.UserData.PlayerItemData.HumanInventory)
                        {
                            if (TargetConditionIdArray[i] == HumanInventoryItemInfo.Id)
                            {
                                ret2 = true;
                                break;
                            }
                            else
                            {
                                ret2 = false;
                            }
                        }

                        // 인벤토리를 확인해봤는데도 TargetConditionIdArray[i]가 없으면
                        if (!ret2)
                        {
                            break;
                        }
                    }
                }
                else if(CurrentState == CurrentState.Cat)
                {
                    for(int i = 0; i < TargetConditionIdArray.Length; i++)
                    {
                        foreach (ItemInfo CatInventoryItemInfo in SaveManager.Instance.UserData.PlayerItemData.CatInventory)
                        {
                            if (TargetConditionIdArray[i] == CatInventoryItemInfo.Id)
                            {
                                ret2 = true;
                                break;
                            }
                            else
                            {
                                ret2 = false;
                            }
                        }

                        if (!ret2)
                        {
                            break;
                        }
                    }
                }
                else if(CurrentState == CurrentState.Dog)
                {
                    for(int i = 0; i < TargetConditionIdArray.Length; i++)
                    {
                        foreach (ItemInfo DogInventoryItemInfo in SaveManager.Instance.UserData.PlayerItemData.DogInventory)
                        {
                            if (TargetConditionIdArray[i] == DogInventoryItemInfo.Id)
                            {
                                ret2 = true;
                                break;
                            }
                            else
                            {
                                ret2 = false;
                            }
                        }

                        if (!ret2)
                        {
                            break;
                        }
                    }
                }

                break;

            case Condition.NoRequiredItem:

                // 이미 보상을 받은적이 없으면  
                if (!SaveManager.Instance.UserData.NpcRewards.ContainsKey(_npcId))
                {
                    ret2 = true;
                }
                else
                {
                    ret2 = false;
                }
                break;

            case Condition.RewardDone:
                // 유저가 보상 아이템을 이미 받은적이 있으면
                if (SaveManager.Instance.UserData.NpcRewards.ContainsKey(_npcId))
                {
                    ret2 = true;
                }
                break;

            case Condition.Card:

                ret2 = GameManager.Instance.Player.CardCheck();
                if (ret2 && !SaveManager.Instance.UserData.NpcRewards.ContainsKey(_npcId)) // 처음 보상을 받았으면 
                {
                    SaveManager.Instance.UserData.NpcRewards[_npcId] = true;
                }
                else
                {
                    ret2 = false;
                }

                break;

            case Condition.QuestNotStarted:
            case Condition.QuestInProgress:
            case Condition.QuestCompleted:

                for (int i = 0; i < TargetConditionIdArray.Length; i++)
                {
                    // TargetConditionIdArray[i]번 퀘스트가 유저 데이터에 있는가? 
                    if (SaveManager.Instance.UserData.QuestInfo.ContainsKey(TargetConditionIdArray[i]))
                    {
                        // Condition이 일치하는가?
                        if (TargetCondition == SaveManager.Instance.UserData.QuestInfo[TargetConditionIdArray[i]])
                        {
                            ret2 = true;
                        }
                        else
                        {
                            ret2 = false;
                        }
                    }

                    else
                    {
                        // TargetConditionIdArray[i]가 진행전 퀘스트라면 (처음 받는 퀘스트라면)
                        if (TargetCondition == Condition.QuestNotStarted)
                        {
                            ret2 = true; // 진행전 퀘스트는 통과 
                        }
                    }
                }

                break;
                
            // 기본 대화 
            default:
                ret2 = true;
                break;
        }

        bool IsRandomNode = false;
        // 조건(Condition)을 만족하면 
        if (ret1 && ret2)
        {
            // 랜덤 대사 세팅 (예정)
            switch (TargetCondition)
            {
                case Condition.QuestInProgress:

                    BaseValue = this.TargetDialogNode.TargetDialogId;

                    Start = (BaseValue / 5) * 5;
                    End = Start + 5;

                    RandomTargetDialogId = Random.Range(Start, End + 1);

                    _randomDialogNode = DataManager.Instance.DialogNodes[RandomTargetDialogId];
                    IsRandomNode = true;
                    break;

                case Condition.QuestCompleted:

                    if(CurrentState == CurrentState.Human)
                    {
                        for(int i = 0; i < TargetConditionIdArray.Length; i++)
                        {
                            if (SaveManager.Instance.UserData.QuestInfo.ContainsKey(TargetConditionIdArray[i]))
                            {
                                if (SaveManager.Instance.UserData.QuestInfo[TargetConditionIdArray[i]] == Condition.QuestCompleted)
                                {
                                    int BaseValue = this.TargetDialogNode.TargetDialogId;

                                    int RandomTargetDialogId = BaseValue + Random.Range(0, 3);

                                    _randomDialogNode = DataManager.Instance.DialogNodes[RandomTargetDialogId];
                                    IsRandomNode = true;
                                }
                            }
                        }
                    }
                    else if(CurrentState == CurrentState.Ghost)
                    {

                    }


                    break;
            }

            if (IsRandomNode)
            {
                DialogManager.Instance.PlayDialogNode(_randomDialogNode);
            }
            else
            {
                // 해당하는 DialogNode를 실행 
                DialogManager.Instance.PlayDialogNode(TargetDialogNode);
            }
        }

        return ret1 && ret2;
    }
}

/*
 
DialogNode 자체를 바꿔야 한다.
PlayDialogNode는 DialogNode의 DialogLineNodeList에 있는 것들을 출력해준다. 
그러니 TargetDialogId를 바꿔봤자 의미없다. 

 
 */