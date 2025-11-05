using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndScene : SceneBase
{
    public override void OnSceneEnter()
    {
        // 씬 전환됐을 때 실행할 코드들
        // 엔드씬에서 필요한 코드들 적기 

        // 배경음악 재생
        //SoundManager.Instance.Set_Bgm(BGM.End); // 음악 세팅
        //SoundManager.Instance.Play_Bgm(); // 음악 재생


    }

    public override void OnSceneExit()
    {
        
    }

    public override void SceneLoading()
    {

    }
}
