using UnityEngine;

public class NPCTrade : MonoBehaviour
{
    public int npcId = 101;
    public int inventoryWidth = 5;
    public int inventoryHeight = 5;

    void Start()
    {
        InventoryManager.Instance.RegisterInventory(npcId, inventoryWidth, inventoryHeight, inventoryWidth, inventoryHeight);
    }

    public void OpenTradeWindow()
    {
        InventoryManager.Instance.ShowTradeWithNPC(CurrentState.Human, npcId);
    }
}