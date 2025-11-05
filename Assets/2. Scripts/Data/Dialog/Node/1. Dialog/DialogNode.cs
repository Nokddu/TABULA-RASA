using System.Collections.Generic;

// 대화 노드 
public class DialogNode
{
    public int TargetDialogId;
    public int ScriptCount;
    public int EndActionCount;

    // 자식 노드 리스트
    private List<EndActionNode> _endActionNodeList;
    public List<DialogLineNode> DialogLineNodeList { get; private set; }

    public DialogNode() { }

    public DialogNode(DialogText DialogText)
    {
        this.TargetDialogId = DialogText.TargetDialogId;
        this.ScriptCount = DialogText.ScriptCount;
        this.EndActionCount = DialogText.EndActionCount;
        _endActionNodeList = new List<EndActionNode>(EndActionCount); 
        DialogLineNodeList = new List<DialogLineNode>(ScriptCount);

        for(int i = 0; i < EndActionCount; i++)
        {
            _endActionNodeList.Add(DataManager.Instance.EndActionNodes[$"{TargetDialogId}_{i}"]);
        }

        for(int i = 0; i < ScriptCount; i++)
        {
            DialogLineNodeList.Add(DataManager.Instance.DialogLineNodes[$"{TargetDialogId}_{i}"]);
        }
    }

    public void SelectLeafNode()
    {
        EndActionTypes ExcutedEndActionType = EndActionTypes.None;

        // endAction 모두 실행 
        foreach (var node in _endActionNodeList)
        {
            ExcutedEndActionType = node.ExcuteNode();
        }

        DialogManager.Instance.EndActionExcuteComplete(ExcutedEndActionType); // EndAction 후처리
    }
}
