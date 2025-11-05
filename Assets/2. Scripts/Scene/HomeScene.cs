using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeScene : SceneBase
{
    public override void OnSceneEnter()
    {
        // 배경음악 세팅
        SoundManager.Instance.Set_Bgm(BGM.PoorTown); 
    }

    public override void OnSceneExit()
    {

    }

    public override void SceneLoading()
    {

    }
}
