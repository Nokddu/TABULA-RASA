public class DialogLineNode
{
    public string TargetDialogId;
    public int Npc1;
    public int Npc1Portrait;
    public int Npc2;
    public int Npc2Portrait;
    public int NpcActive;
    public int Background;
    public string Scripts;
    public int Speaker;
    public float TypingSpeed;

    public DialogLineNode(DialogLine DialogLine)
    {
        this.TargetDialogId = DialogLine.TargetDialogId;
        this.Npc1 = DialogLine.Npc1;
        this.Npc1Portrait = DialogLine.Npc1Portrait;
        this.Npc2 = DialogLine.Npc2;
        this.Npc2Portrait = DialogLine.Npc2Portrait;
        this.NpcActive = DialogLine.NpcActive;
        this.Background = DialogLine.Background;
        this.Scripts = DialogLine.Scripts;
        this.Speaker = DialogLine.Speaker;
        this.TypingSpeed = DialogLine.TypingSpeed;
    }
}
