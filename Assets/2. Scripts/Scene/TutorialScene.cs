using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialScene : SceneBase
{
    public override void OnSceneEnter()
    {
        // 배경음악 재생( 에셋구할때까지 )
        SoundManager.Instance.Set_Bgm(BGM.Tutorial);
    }

    public override void OnSceneExit()
    {
        
    }

    public override void SceneLoading()
    {

    }
}
