using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager : Singleton<SaveManager>
{
    public UserData UserData;
    public SystemData SystemData;

    /// <summary>
    /// 로드할 파일 존재시 true
    /// </summary>
    [HideInInspector]
    public bool IsLoaded;

    /// <summary>
    /// 데이터 로드 완료
    /// </summary>
    [HideInInspector]
    public bool IsLoadCompleted;

    private string _userPath;
    private string _systemPath;
    private DataManager _data;

    protected override void Awake()
    {
        base.Awake();

#if UNITY_EDITOR
        _userPath = Path.Combine(Application.dataPath, "5. Data/Json/TestUserData.json");
        _systemPath = Path.Combine(Application.dataPath, "5. Data/Json/TestSystemData.json");
#else
        _userPath = Path.Combine(Application.persistentDataPath, "UserData.json");
        _systemPath = Path.Combine(Application.persistentDataPath, "SystemData.json");
#endif
    }

    private void Start()
    {
        _data = DataManager.Instance;

        Load_Data();

        IsLoadCompleted = true;
    }

    public void Load_Data()
    {
        // === 파일 존재시 ===
        if (File.Exists(_userPath) && File.Exists(_systemPath))
        {
            var loadUserData = File.ReadAllText(_userPath);
            var loadSystemData = File.ReadAllText(_systemPath);

            UserData = JsonConvert.DeserializeObject<UserData>(loadUserData, new JsonSerializerSettings() { ObjectCreationHandling = ObjectCreationHandling.Replace });
            SystemData = JsonConvert.DeserializeObject<SystemData>(loadSystemData, new JsonSerializerSettings() { ObjectCreationHandling = ObjectCreationHandling.Replace });
            
            IsLoaded = true;
        }
        else // === 없으면 새로만듬 ===
        {
            SystemData = new SystemData
            {
                ScreenSize = "Full",
                BgmVolume = 0.25f,
                SfxVolume = 0.5f,
            };

            ReLoad_Data();
        }
    }

    public void ReLoad_Data()
    {
        if (IsLoadCompleted)
        {
            _data.Re_Data_Set();
        }

        IsLoadCompleted = false;

        UserData = new UserData
        {
            SceneNumber = 0,
            PlayerPosX = 0,
            PlayerPosY = -0.5f,
            PlayerVessel = _data.VesselPlayer.PoolList,
            Level = 0,
            Players = _data.PlayerSO.PlayerData,
            PlayerItemData = new PlayerItemData(),
            QuestInfo = new Dictionary<int, Condition>(),
            NpcRewards = new Dictionary<int, bool>(),
            DoorInteracted = new Dictionary<int, bool>(),
            CurrentMilestone = "여기는 어디지...?",
            SaveImgs = new List<string>(new string[4])
        };

        UserData.Players.PoolTag = null;

        string jsonUser = JsonConvert.SerializeObject(UserData);
        string jsonSystem = JsonConvert.SerializeObject(SystemData);

        File.WriteAllText(_userPath, jsonUser);
        File.WriteAllText(_systemPath, jsonSystem);

        DataManager.Instance.Init_Ground_Item_Data();

        Save_Data(UserData, SystemData);

        IsLoadCompleted = true;

        GameManager.Instance.PlayerPos = new(UserData.PlayerPosX, UserData.PlayerPosY);
    }

    public void Save_Data(UserData user, SystemData system)
    {
        var userData = JsonConvert.SerializeObject(user);
        var systemData = JsonConvert.SerializeObject(system);

        File.WriteAllText(_userPath, userData);
        File.WriteAllText(_systemPath, systemData);
    }

    public void Save_System(SystemData system)
    {
        var systemData = JsonConvert.SerializeObject(system);

        File.WriteAllText(_systemPath, systemData);
    }

}
