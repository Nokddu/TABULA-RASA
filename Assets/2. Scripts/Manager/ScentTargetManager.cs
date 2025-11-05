using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ScentTargetManager
{
   public static List<ScentTarget> AllTargets = new List<ScentTarget>();

    public static void Register(ScentTarget target)
    {
        if (!AllTargets.Contains(target))
        {
            AllTargets.Add(target);
        }
    }

    public static void Unregister(ScentTarget target)
    {
        if (AllTargets.Contains(target))
        {
            AllTargets.Remove(target);
        }
    }
    public static bool DoTargetsExist(int scentId)
    {
        foreach (ScentTarget target in AllTargets)
        {
            if (target.ScentId == scentId)
            {
                return true;
            }
        }
        return false;
    }
}
