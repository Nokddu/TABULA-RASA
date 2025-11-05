public class MileStoneNode
{
    public string TargetDialogId;
    public string MileStone;

    public MileStoneNode(MileStones MileStone)
    {
        this.TargetDialogId = MileStone.TargetDialogId;
        this.MileStone = MileStone.MileStone;
    }
}
