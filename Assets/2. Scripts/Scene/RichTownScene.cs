using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RichTownScene : SceneBase
{
    public override void OnSceneEnter()
    {
        // 배경음악 재생
        SoundManager.Instance.Set_Bgm(BGM.RichTown);
    }

    public override void OnSceneExit()
    {

    }

    public override void SceneLoading()
    {

    }
}
