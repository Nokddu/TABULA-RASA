using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPoolSO", menuName = "Object Pool Settings")]
public class Pools : ScriptableObject
{
    public List<PoolData> PoolList;
}
