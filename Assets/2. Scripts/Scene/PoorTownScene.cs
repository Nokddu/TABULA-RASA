using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoorTownScene : SceneBase
{
    public override void OnSceneEnter()
    {
        // 배경음악 재생
        SoundManager.Instance.Set_Bgm(BGM.PoorTown); 
    }

    public override void OnSceneExit()
    {

    }

    public override void SceneLoading()
    {

    }
}
