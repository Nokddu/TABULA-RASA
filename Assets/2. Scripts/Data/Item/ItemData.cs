using System;

/// <summary>
/// (상태)얻지 않음, 얻음, 사용함
/// </summary>
    [Flags]
    public enum ItemFlags
    {
        None = 1 << 0,         // 0001 (1)
        isAcquired = 1 << 1   // 0010 (2)  
    }

[Serializable]
public class ItemData
{
    // === 아이템 정보 ===
    public int Id;
    public string Name;
    public string Description;
    public string ImagePath;
    public int Width;
    public int Height;
    public string[] shape;
    public bool IsDroppable = true;
}

/// <summary>
/// 게임내에서 바뀌는 아이템
/// </summary>
[Serializable]
public class ItemInfo
{
    public int Id;
    public int Key;
    public ItemFlags Flags;
    public int Rotation;
    public float PosX;
    public float PosY;
    public bool IsInInventory;
    public int GridX;
    public int GridY;

    public ItemInfo(int targetid, int targetkey) 
    {
        Id = targetid;
        Key = targetkey;
    }
}

/// <summary>
/// 필드에 생성될 아이템
/// </summary>
[Serializable]
public class InitGroundItemInfo
{
    public int Id;
    public int AreaIndex;
    public float PosX;
    public float PosY;
}