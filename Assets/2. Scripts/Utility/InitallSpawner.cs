using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitallSpawner : Singleton<InitallSpawner>
{
    [Header("Tag")]
    public List<string> VesselPoolTag;

    private SaveManager _save;
    private ObjectPool _pool;

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => SaveManager.Instance.IsLoadCompleted);

        _save = SaveManager.Instance;
        _pool = ObjectPool.Instance;
    }

    // === 빙의체 이동 ===
    public void Generate_Area_Vessel(List<PoolData> saveData)
    {
        foreach (var loadPool in saveData)
        {
            if (loadPool.Tag == VesselPoolTag[loadPool.PrefabsIndex] && loadPool.AreaIndex == _save.UserData.SceneNumber)
            {
                if (loadPool.AreaIndex != -1)
                {
                    Vector2 Pos = new(loadPool.PosX, loadPool.PosY);

                    Set_Position(loadPool.PrefabsIndex, Pos);
                }
            }
            else
            {
                Off_Pool(loadPool.PrefabsIndex);
            }
        }
    }

    // === 위치 조절 ===
    public void Set_Position(int num, Vector2 pos)
    {
        _pool.VesselChildrenList[num].transform.position = pos;

        _pool.VesselChildrenList[num].SetActive(true);
    }

    // === 비활성 화 ===
    public void Off_Pool(int num)
    {
        _pool.VesselChildrenList[num].SetActive(false);
    }
}
