using System;

[Serializable]
public class Dialog
{
    public int Id;
    public CurrentState State;
    public string Condition;
    public int TargetDialogId;
}
