using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    private InventoryItem[,] grid;
    private int gridWidth, gridHeight, usableWidth, usableHeight;
    public CurrentState OwnerState { get; set; }

    public Inventory(int gw, int gh, int uw, int uh, CurrentState owner)
    {
        gridWidth = gw; gridHeight = gh; usableWidth = uw; usableHeight = uh;
        this.OwnerState = owner;
        grid = new InventoryItem[gridWidth, gridHeight];
    }

    public InventoryItem GetItem(int x, int y)
    {
        if (x < 0 || y < 0 || x >= gridWidth || y >= gridHeight) return null;
        return grid[x, y];
    }

    public bool CanPlaceItem(InventoryItem item, int startX, int startY)
    {
        if (startX < 0 || startY < 0 || startX + item.width > usableWidth || startY + item.height > usableHeight) return false;
        for (int y = 0; y < item.height; y++)
        {
            for (int x = 0; x < item.width; x++)
            {
                if (item.occupiedCells[y, x] && grid[startX + x, startY + y] != null) return false;
            }
        }
        return true;
    }

    public bool PlaceItem(InventoryItem item, int startX, int startY)
    {
        if (!CanPlaceItem(item, startX, startY)) return false;
        item.GridX = startX;
        item.GridY = startY;
        for (int y = 0; y < item.height; y++)
        {
            for (int x = 0; x < item.width; x++)
            {
                if (item.occupiedCells[y, x]) grid[startX + x, startY + y] = item;
            }
        }
        return true;
    }
    public InventoryItem RemoveItem(int x, int y)
    {
        InventoryItem itemToRemove = grid[x, y];
        if (itemToRemove == null) return null;

        for (int itemY = 0; itemY < itemToRemove.height; itemY++)
        {
            for (int itemX = 0; itemX < itemToRemove.width; itemX++)
            {
                if (itemToRemove.occupiedCells[itemY, itemX])
                {
                    grid[itemToRemove.GridX + itemX, itemToRemove.GridY + itemY] = null;
                }
            }
        }
        return itemToRemove;
    }

    public List<InventoryItem> GetAllItems()
    {
        var allItems = new List<InventoryItem>();
        var processedItems = new HashSet<InventoryItem>();

        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                InventoryItem item = grid[x, y];
                if (item != null && !processedItems.Contains(item))
                {
                    allItems.Add(item);
                    processedItems.Add(item);
                }
            }
        }
        return allItems;
    }
    public bool CheckRequirements(List<QuestItemRequirement> requirements)
    {
        var itemCounts = new Dictionary<int, int>();

        foreach (var item in GetAllItems())
        {
            if (item != null)
            {
                if (!itemCounts.ContainsKey(item.ItemData.Id))
                {
                    itemCounts[item.ItemData.Id] = 0;
                }
                itemCounts[item.ItemData.Id]++;
            }
        }

        foreach (var req in requirements)
        {
            if (!itemCounts.ContainsKey(req.itemID) || itemCounts[req.itemID] < req.quantity)
            {
                return false;
            }
        }

        return true;
    }
    public bool AddItemToFirstAvailableSlot(InventoryItem item)
    {
        for (int y = 0; y < usableHeight; y++)
        {
            for (int x = 0; x < usableWidth; x++)
            {
                if (CanPlaceItem(item, x, y)) { PlaceItem(item, x, y); return true; }
            }
        }
        return false;
    }
    public void Clear() 
    { 
        grid = new InventoryItem[gridWidth, gridHeight]; 
    }

    public void PrintGridStatus()
    {
        string gridContent = $"--- {OwnerState} 인벤토리 상태 ---\n";
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                // 칸이 비어있으면 O, 차있으면 X로 표시
                gridContent += (grid[x, y] == null) ? "[ O ]" : "[ X ]";
            }
            gridContent += "\n"; // 한 줄 띄우기
        }
        Debug.Log(gridContent);
    }
}