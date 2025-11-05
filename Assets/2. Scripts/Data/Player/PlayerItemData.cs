using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class PlayerItemData
{
    public int ItemKey = 0;
    public List<ItemInfo> HumanInventory;
    public List<ItemInfo> CatInventory;
    public List<ItemInfo> DogInventory;
    // scene 인덱스
    public Dictionary<int, List<ItemInfo>> GroundItem;
    /// <summary>
    /// 아이템 데이터를 받을꺼면 생성자가 아닌 아래의 GenerateNewItemInfo 메서드를 이용해야함
    /// </summary>
    public PlayerItemData()
    {
        GroundItem = new Dictionary<int, List<ItemInfo>>();
        HumanInventory = new List<ItemInfo>();
        CatInventory = new List<ItemInfo>();
        DogInventory = new List<ItemInfo>();
    }

    public ItemInfo GenerateNewItemInfo(int targetId)
    {
        ItemInfo ret = new(targetId, ItemKey);
        ItemKey++;
        return ret;
    }

    public void TransferItem(ItemInfo item, CurrentState fromState, CurrentState toState) //아이템을 다른 리스트로 옮기는 함수
    {
        List<ItemInfo> fromList = GetInventoryList(fromState);
        List<ItemInfo> toList = GetInventoryList(toState);

        if (fromList != null && toList != null && fromList.Contains(item))
        {
            fromList.Remove(item);
            toList.Add(item);
        }
    }

    public void ItemGet(ItemInfo item, CurrentState state)
    {
        switch (state)
        {
            case CurrentState.Human:
                HumanInventory.Add(item);
                break;
            case CurrentState.Cat:
                CatInventory.Add(item);
                break;
            case CurrentState.Dog:
                DogInventory.Add(item);
                break;
        }
        //item.IsInInventory = true;
        item.IsInInventory = true;
        GroundItem[SaveManager.Instance.UserData.SceneNumber].Remove(item);
        //    현재 스테이지 변수
        // areaindex 를 받아온다
    }

    public ItemInfo DropItem(ItemInfo item, CurrentState state, Vector2 position)
    {
        item.IsInInventory = false;
        item.PosX = position.x;
        item.PosY = position.y;
        item.GridX = 0; // 인벤토리 소속이 아니므로 그리드 좌표 초기화
        item.GridY = 0;

        int currentSceneIndex = SaveManager.Instance.UserData.SceneNumber; 
        if (!GroundItem.ContainsKey(currentSceneIndex))
        {
            GroundItem[currentSceneIndex] = new List<ItemInfo>();
        }
        GroundItem[currentSceneIndex].Add(item);

        // 해당 인벤토리 리스트에서 아이템 제거
        GetInventoryList(state)?.Remove(item);

        return item;
    }

    public List<ItemInfo> GetInventoryList(CurrentState state)
    {
        switch (state)
        {
            case CurrentState.Human: 
                return HumanInventory;
            case CurrentState.Cat: 
                return CatInventory;
            case CurrentState.Dog: 
                return DogInventory;
            default: return null;
        }
    }

    public void RemoveItemFromInventory(ItemInfo itemToRemove, CurrentState ownerState)
    {
        List<ItemInfo> targetList = GetInventoryList(ownerState);
        if (targetList == null) return;

        targetList.RemoveAll(item => item.Key == itemToRemove.Key);
    }
}
