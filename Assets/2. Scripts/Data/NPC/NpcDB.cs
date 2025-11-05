using System.Collections.Generic;

public class NpcDB 
{
    public readonly Dictionary<int, List<NpcData>> NpcDatas;

    public NpcDB(Npcs NpcsSO)
    {
        NpcDatas = new();
        if (NpcsSO != null && NpcsSO.NpcData != null)
        {
            foreach (NpcData NpcData in NpcsSO.NpcData)
            {
                if (NpcData != null)
                {
                    if (!NpcDatas.ContainsKey(NpcData.AreaIndex))
                    {
                        NpcDatas[NpcData.AreaIndex] = new List<NpcData>();
                    }

                    NpcDatas[NpcData.AreaIndex].Add(NpcData);
                }
            }
        }
    }
}
