using System.Collections;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public Player Player;
    public GameObject FiledItems;
    [Header("Potal")]
    public int Index;
    public PortalDirection PortalInfo;
    public Vector2 PlayerPos;

    public bool HasSavedState { get; private set; } = false;

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => SaveManager.Instance.IsLoadCompleted);

        PlayerPos = new(SaveManager.Instance.UserData.PlayerPosX, SaveManager.Instance.UserData.PlayerPosY);
    }

    public void Clear_Saved_State()
    {
        HasSavedState = false;
    }

    public void Set_Player()
    {
        if ( Player == null)
        {
            return;
        }
        GameObject TransformPos = GameObject.FindWithTag("Player");

        InitallSpawner.Instance.Generate_Area_Vessel(SaveManager.Instance.UserData.PlayerVessel);

        Player.Start_Possess();

        TransformPos.transform.position = PlayerPos;

        PlayerPos = new(SaveManager.Instance.UserData.PlayerPosX, SaveManager.Instance.UserData.PlayerPosY);
    }

    public void Object_Destroy(GameObject go)
    {
        Destroy(go);
    }

    public void Spawn_Field_Item(ItemInfo itemData)
    {
        Vector2 spawnPos = new(itemData.PosX, itemData.PosY);
        GameObject itemObject = Instantiate(FiledItems, spawnPos, Quaternion.identity);

        FieldItem fieldItemComponent = itemObject.GetComponent<FieldItem>();

        if (fieldItemComponent != null)
        {
            fieldItemComponent.Init(itemData);
        }
    }
    public bool DropItemToField(InventoryItem item, CurrentState fromState)
    {
        SoundManager.Instance.Play_Sfx(SFX.Throw);

        if (item == null || Player == null) return false;

        if (item.ItemData != null && !item.ItemData.IsDroppable)
        {
            NotificationManager.Instance.ShowCannotDropMessage();
            return false;
        }

        Vector2 dropPosition = Player.transform.position;

        SaveManager.Instance.UserData.PlayerItemData.DropItem(item.ItemInfo, fromState, dropPosition);

        Spawn_Field_Item(item.ItemInfo, dropPosition);

        Player.UpdateScentTracking();

        return true;
    }
    public void Spawn_Field_Item(ItemInfo itemInfo, Vector3 position)
    {
        if (FiledItems == null)
        {
            Debug.LogError("FieldItem Prefab is not assigned in GameManager!");
            return;
        }
        GameObject itemGO = Instantiate(FiledItems, position, Quaternion.identity);
        FieldItem fieldItem = itemGO.GetComponent<FieldItem>();
        if (fieldItem != null)
        {
            fieldItem.Init(itemInfo);
        }
    }

    public void Register_Player(Player playerInstance) { this.Player = playerInstance; }
}
