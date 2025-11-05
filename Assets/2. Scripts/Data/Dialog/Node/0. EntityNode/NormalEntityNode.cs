using System.Collections.Generic;

// 일반 NPC/오브젝트 노드 
public class NormalEntityNode : EntityNode
{
    public NormalEntityNode(int Id) : base(Id)
    {
        
    }

    // NormalEntityNode의 Leaf Node 초기화.
    public void AddDialogNode(List<Dialog> DialogList)
    {
        foreach(var Dialog in DialogList)
        {
            _dialogNodeList.Add(new NormalEntityDialogNode(Dialog));
        }
    }
}
