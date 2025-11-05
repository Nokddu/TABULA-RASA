using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SceneBase 
{
    public abstract void SceneLoading();

    // Start와 같은 역할. 씬 진입 시점. 
    public abstract void OnSceneEnter();

    // OnDestroy와 비슷한 역할. 씬 전환 할 때, 씬이 비활성화 될때. 
    public abstract void OnSceneExit();
}
