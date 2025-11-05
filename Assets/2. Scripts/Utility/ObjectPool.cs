using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : Singleton<ObjectPool>
{
    [SerializeField]
    private List<GameObject> _vesselPrefabs;

    [SerializeField]
    private List<GameObject> _ambiencePrefabs;

    private Dictionary<string, Queue<GameObject>> _vesselDictionary;
    private Dictionary<string, Queue<GameObject>> _ambienceDctionary;

   [HideInInspector]
    public List<GameObject> VesselChildrenList = new();

    protected override void Awake()
    {
        base.Awake();

        _vesselDictionary = new Dictionary<string, Queue<GameObject>>();
        _ambienceDctionary = new Dictionary<string, Queue<GameObject>>();
    }

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => SaveManager.Instance.IsLoadCompleted);

        foreach (var pool in DataManager.Instance.VesselPlayer.PoolList)
        {
            Queue<GameObject> objectPool = new();
            _vesselDictionary.Add(pool.Tag, objectPool);

            GameObject obj = Instantiate(_vesselPrefabs[pool.PrefabsIndex]);

            obj.SetActive(false);
            obj.transform.SetParent(this.transform);
            objectPool.Enqueue(obj);

            Get_Pool_Vessel(pool.Tag);

            VesselChildrenList.Add(obj);
        }

        foreach (var pool in DataManager.Instance.Ambience.PoolList)
        {
            Queue<GameObject> objectPool = new();
            _ambienceDctionary.Add(pool.Tag, objectPool);

            GameObject ambienceObj = Instantiate(_ambiencePrefabs[0]);
            ambienceObj.SetActive(false);
            ambienceObj.transform.SetParent(this.transform);

            objectPool.Enqueue(ambienceObj);
        }
    }

    // === 빙의체 생성 ===
    public GameObject Get_Pool_Vessel(string tag)
    {
        GameObject objectToSpawn = _vesselDictionary[tag].Dequeue();

        objectToSpawn.SetActive(true);

        return objectToSpawn;
    }

    // === 빙의체에서 내릴때 ===
    public GameObject Off_Pool_Vessel(string tag, Vector2 pos)
    {
        GameObject objectToSpawn = _vesselDictionary[tag].Dequeue();

        objectToSpawn.SetActive(true);

        objectToSpawn.transform.position = pos;

        return objectToSpawn;
    }

    // === 풀 반환 ===
    public void Return_To_Pool(string tag, GameObject objectToReturn)
    {
        if (!_vesselDictionary.ContainsKey(tag))
        {
            Debug.Log($"현재 : {tag} 가 없습니다.");
            return;
        }

        objectToReturn.SetActive(false);
        _vesselDictionary[tag].Enqueue(objectToReturn);
    }

    // === 환경음 생성 ===
    public GameObject Get_Pool_Ambience(string tag, Ambience sfx)
    {
        if (!_ambienceDctionary.ContainsKey(tag))
        {
            return null;
        }

        Queue<GameObject> poolQueue = _ambienceDctionary[tag];

        GameObject objectToSpawn;

        if (poolQueue.Count == 0)
        {
            GameObject newObj = Instantiate(_ambiencePrefabs[0]);

            newObj.transform.SetParent(this.transform);

            objectToSpawn = newObj;
        }
        else
        {
            objectToSpawn = poolQueue.Dequeue();
        }

        objectToSpawn.SetActive(true);

        if (_ambienceDctionary.ContainsKey(tag))
        {
            SoundManager.Instance.Trace_Ambience(tag, objectToSpawn, sfx);
        }

        return objectToSpawn;
    }

    public void Return_To_Ambience(string tag, GameObject objectToReturn)
    {
        if (!_ambienceDctionary.ContainsKey(tag))
        {
            Debug.Log($"현재 : {tag} 가 없습니다.");
            return;
        }

        objectToReturn.SetActive(false);
        _ambienceDctionary[tag].Enqueue(objectToReturn);
    }
}
