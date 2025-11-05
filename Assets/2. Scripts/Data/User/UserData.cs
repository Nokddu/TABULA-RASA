using System;
using System.Collections.Generic;

/// <summary>
/// 업, 자아단계, 플레이어 위치, 플레이어 정보
/// 새롭게 저장할 SO추가 할때마다 리스트 만들어야함
/// </summary>    
[Serializable]
public class UserData 
{
    public int SceneNumber;
    public float PlayerPosX;
    public float PlayerPosY;
    public List<PoolData> PlayerVessel;
    public int Level;
    public PlayerData Players;
    public PlayerItemData PlayerItemData;
    public Dictionary<int, Condition> QuestInfo;
    public Dictionary<int, bool> NpcRewards;
    public Dictionary<int, bool> DoorInteracted;
    public string CurrentMilestone;
    public string PlayerImg;
    public List<string> SaveImgs;
}
