using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScentTarget : MonoBehaviour
{
    public int ScentId;

    private void OnEnable()
    {
        ScentTargetManager.Register(this);
    }

    private void OnDisable()
    {
        ScentTargetManager.Unregister(this);
    }
}
