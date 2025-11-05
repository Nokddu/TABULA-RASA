using System.Collections.Generic;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    public Items ItemSO;
    public Quests QuestSO;
    public Npcs NpcSO;
    public Dialogs DialogsSO;
    public PlayerSO PlayerSO;
    public Pools VesselSO;
    public Pools AmbienceSO;

    [HideInInspector]
    public Pools VesselPlayer;

    [HideInInspector]
    public Pools Ambience;

    public ItemDB ItemDB { get; private set; }

    public QuestDB QuestDB { get; private set; }

    public NpcDB NpcDB { get; private set; }

    public DialogDB DialogDB { get; private set; }

    // Node
    public Dictionary<string, EndActionNode> EndActionNodes { get; private set; } // 대화 이후 행동 노드 
    public Dictionary<int, List<DialogImage>> ImageNodes { get; private set; } // 이미지 노드 
    public Dictionary<string, DialogLineNode> DialogLineNodes { get; private set; } // 대화(한줄) 노드 
    public Dictionary<string, MileStoneNode> MileStoneNodes { get; private set; } // 현재 목표 노드 
    public Dictionary<int, NpcNode> NpcNodes { get; private set; } // Npc 노드 
    public Dictionary<int, DialogNode> DialogNodes { get; private set; } // 대화 노드 
    public Dictionary<int, EntityNode> EntityNodes { get; private set; } // 퀘스트, 일반 NPC/오브젝트 노드 

    /// <summary>
    /// SO의 클론을 저장중
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

        ItemDB = new ItemDB(Instantiate(ItemSO));
        QuestDB = new QuestDB(Instantiate(QuestSO));
        NpcDB = new NpcDB(Instantiate(NpcSO));
        DialogDB = new DialogDB(Instantiate(DialogsSO));
        VesselPlayer = Instantiate(VesselSO);
        Ambience = Instantiate(AmbienceSO);

        InitNodes();
    }

    public void Init_Ground_Item_Data()
    {
        var groundItem = SaveManager.Instance.UserData.PlayerItemData.GroundItem;
        foreach (var data in ItemDB.InitGroundInfo)
        {
            if (!groundItem.TryGetValue(data.AreaIndex, out List<ItemInfo> infos))
            {
                groundItem[data.AreaIndex] = infos = new List<ItemInfo>();
            }

            var target = SaveManager.Instance.UserData.PlayerItemData.GenerateNewItemInfo(data.Id);

            target.PosX = data.PosX;
            target.PosY = data.PosY;

            infos.Add(target);
        }
    }

    private void InitNodes()
    {
        // EndActionNodes
        EndActionNodes = new Dictionary<string, EndActionNode>();
        foreach(string Id in DialogDB.EndActions.Keys)
        {
            EndActionNodes[Id] = new EndActionNode(DialogDB.EndActions[Id]);
        }

        // ImageNodes
        ImageNodes = new Dictionary<int, List<DialogImage>>();
        foreach(int Id in DialogDB.DialogImages.Keys)
        {
            foreach(DialogImage DialogImage in DialogDB.DialogImages[Id])
            {
                if (!ImageNodes.ContainsKey(Id))
                {
                    ImageNodes[Id] = new List<DialogImage>();
                }
                ImageNodes[Id].Add(DialogImage);
            }
        }

        // NpcNodes
        NpcNodes = new Dictionary<int, NpcNode>();
        foreach(int AreaIndex in NpcDB.NpcDatas.Keys)
        {
            foreach(NpcData data in NpcDB.NpcDatas[AreaIndex])
            {
                NpcNodes[data.Id] = new NpcNode(data.Id, data.Name);
            }
        }

        // DialogLineNodes
        DialogLineNodes = new Dictionary<string, DialogLineNode>();
        foreach(string Id in DialogDB.DialogLines.Keys)
        {
            DialogLineNodes[Id] = new DialogLineNode(DialogDB.DialogLines[Id]);
        }

        // MilestoneNodes
        MileStoneNodes = new Dictionary<string, MileStoneNode>();
        foreach(string Id in DialogDB.MileStones.Keys)
        {
            MileStoneNodes[Id] = new MileStoneNode(DialogDB.MileStones[Id]);
        }

        // DialogNodes
        DialogNodes = new Dictionary<int, DialogNode>();
        foreach (int Id in DialogDB.DialogTexts.Keys)
        {
            DialogNodes[Id] = new DialogNode(DialogDB.DialogTexts[Id]);
        }

        // NormalEntityNodes
        EntityNodes = new Dictionary<int, EntityNode>();
        foreach(var now in DialogDB.EntityDialogMap)
        {
            var newNode = new NormalEntityNode(now.Key);
            newNode.AddDialogNode(now.Value);
            EntityNodes[now.Key] = newNode;
        }
    }

    public void Re_Data_Set()
    {
        ItemDB = new ItemDB(Instantiate(ItemSO));
        QuestDB = new QuestDB(Instantiate(QuestSO));
        NpcDB = new NpcDB(Instantiate(NpcSO));
        DialogDB = new DialogDB(Instantiate(DialogsSO));
        VesselPlayer = Instantiate(VesselSO);
        Ambience = Instantiate(AmbienceSO);
    }
}
