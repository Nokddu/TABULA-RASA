using UnityEngine;

public class FieldItem : MonoBehaviour
{
    public ItemInfo TargetItemInfo;
    private ItemData itemData;
    public void Init(ItemInfo target)
    {
        TargetItemInfo = target;
        itemData = DataManager.Instance.ItemDB.Get(TargetItemInfo.Id);
    }
    private void OnMouseEnter()
    {
        if (TooltipManager.Instance != null && itemData != null && InventoryManager.Instance.CurrentDraggedItem == null)
        {
            TooltipManager.Instance.ShowTooltip(
                itemData.Name,
                itemData.Width,
                itemData.Height
            );
        }
    }
    private void OnMouseExit()
    {
        if (TooltipManager.Instance != null)
        {
            TooltipManager.Instance.HideTooltip();
        }
    }
}
