using System;

[Serializable]
public class QpcDialog
{
    public int Id;
    public CurrentState State;
    public int QuestId;
    public QuestFlag Quests;
    public int TargetDialogId;
}
