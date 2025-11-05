using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestDB
{
    public readonly Dictionary<int, QuestData> QuestDatas;

    public QuestDB(Quests QuestsSO)
    {
        QuestDatas = new Dictionary<int, QuestData>();
        if (QuestsSO != null && QuestsSO.QuestData != null)
        {
            foreach (QuestData data in QuestsSO.QuestData)
            {
                if (data != null)
                {
                    QuestDatas[data.Id] = data;
                }
            }
        }
    }
    /// <summary>
    /// 일치하는 id값의 정보를 가져옵니다.
    /// </summary>
    /// <param name="Id"></param>
    /// <returns></returns>
    public QuestData Get(int Id)
    {
        QuestDatas.TryGetValue(Id, out QuestData QuestData);

        return QuestData;
    }
}
