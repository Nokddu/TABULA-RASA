using UnityEngine;

public class ItemSpawnManager : Singleton<ItemSpawnManager>
{
    public GameObject FiledItems; 

    public void GenerateAreaItem(int Areaindex, Transform Parents)
    {
        if (SaveManager.Instance.UserData.PlayerItemData.GroundItem.ContainsKey(Areaindex))
        {
            foreach (ItemInfo ItemInfo in SaveManager.Instance.UserData.PlayerItemData.GroundItem[Areaindex])
            {
                GenerateGroundItem(Parents, ItemInfo);
            }
        }
    }

    public void GenerateGroundItem(Transform Parents, ItemInfo ItemInfo)
    {
        FieldItem Nowitem = Instantiate(FiledItems, new Vector2(ItemInfo.PosX, ItemInfo.PosY), Quaternion.identity, Parents).GetComponent<FieldItem>();
        Nowitem.Init(ItemInfo);
    }

    public void GenerateAreaNpc(int Areaindex, Transform Parents)
    {
        if (DataManager.Instance.NpcDB.NpcDatas.ContainsKey(Areaindex))
        {
            foreach (NpcData NpcData in DataManager.Instance.NpcDB.NpcDatas[Areaindex])
            {
                if (SaveManager.Instance.UserData.DoorInteracted.ContainsKey(NpcData.Id)) continue;

                if (NpcData.Id == 0) continue;

                Npc Npc = ResourceManager.Instance.Create_Character<Npc>($"{Prefab.NPC}/{Prefab.NPC}_{NpcData.Id}", new Vector2(NpcData.PosX, NpcData.PosY));
                Npc.Init(NpcData);
            }
        }
    }
}
