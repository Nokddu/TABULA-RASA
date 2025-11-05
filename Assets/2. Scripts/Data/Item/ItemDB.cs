using System.Collections.Generic;

public class ItemDB 
{
    public readonly Dictionary<int, ItemData> Items;
    public readonly List<InitGroundItemInfo> InitGroundInfo;
    
    public ItemDB(Items so)
    {
        Items = new Dictionary<int, ItemData>();
        if (so != null && so.Item != null)
        {
            foreach (var item in so.Item)
            {
                if (item != null)
                {
                    Items[item.Id] = item;
                }
            }
        }

        InitGroundInfo = new List<InitGroundItemInfo>();
        if (so != null && so.InitGroundItemInfo != null)
        {
            foreach (var item in so.InitGroundItemInfo)
            {
                if (item != null)
                {
                    InitGroundInfo.Add(item);
                }
            }
        }
    }
    /// <summary>
    /// 일치하는 id값의 정보를 가져옵니다.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public ItemData Get(int id)
    {
        Items.TryGetValue(id, out ItemData data);

        return data;
    }
}
