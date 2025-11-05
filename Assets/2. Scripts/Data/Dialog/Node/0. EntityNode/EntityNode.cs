using System.Collections.Generic;

// QpcEntityNode, NormalEntityNode 부모 클래스 
public abstract class EntityNode 
{
    protected int Id;

    // 자식 노드 리스트
    protected List<IEntityDialogNode> _dialogNodeList;

    public EntityNode(int Id)
    {
        this.Id = Id;
        _dialogNodeList = new List<IEntityDialogNode>();
    }

    // EntityNode의 DialogNode들을 하나씩 검사 => Id에 맞는 DialogNode를 찾기 위함. 
    public void SelectLeafNode(CurrentState CurrentState)
    {
        foreach (var node in _dialogNodeList)
        {
            if (node.TrySelectNode(CurrentState))
            {
                return; 
            }
        }
    }

    public void SelectLeafNode()
    {
        foreach (var node in _dialogNodeList)
        {
            if (node.TrySelectNode(CurrentState.All))
            {
                return; 
            }
        }
    }
}
