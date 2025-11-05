using System.Collections.Generic;
using UnityEngine;

[ExcelAsset(AssetPath = "5. Data/SO")]
public class Items : ScriptableObject
{
    public List<ItemData> Item;
    public List<InitGroundItemInfo> InitGroundItemInfo;
}
