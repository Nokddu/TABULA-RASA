using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum QuestFlag
{
    QuestNotStarted,
    QuestInProgress,
    QuestCompleted
}

[Serializable]
public class QuestData 
{
    public int Id;
    public string Name;
    public string Description;

    [Tooltip("excel 사용방법 : 1101,1102")]
    public string RequiredItemID;
    [Tooltip("excel 사용방법 : 1,2")]
    public string Count;
}
/// <summary>
/// 가변 데이터
/// </summary>
[Serializable]
public class QuestInfo
{
    public int Id;
    public Condition QuestFlag;
}