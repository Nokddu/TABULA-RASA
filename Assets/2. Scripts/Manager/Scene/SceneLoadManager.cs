using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadManager : Singleton<SceneLoadManager>
{
    public GameObject GameOverPanel;
    private Dictionary<SceneType, SceneBase> _scenes = new Dictionary<SceneType, SceneBase>();

    private SceneBase _currentScene;
    private SceneBase _prevScene;

    private Coroutine _loadingCoroutine;

    [SerializeField] private Animator _animator;
    [SerializeField] private float transitionTime;

    // 씬이 시작됐을 때, 필요한 씬들을 등록
    protected override void Awake()
    {
        base.Awake();

        _animator = GetComponentInChildren<Animator>();

        _scenes.Add(SceneType.Title, new TitleScene());
        _scenes.Add(SceneType.Tutorial, new TutorialScene());
        _scenes.Add(SceneType.Home, new HomeScene());
        _scenes.Add(SceneType.PoorTown, new PoorTownScene());
        _scenes.Add(SceneType.RichTown, new RichTownScene());
        _scenes.Add(SceneType.RuralTown, new RuralTownScene());
        _scenes.Add(SceneType.End, new EndScene());
    }

    // 씬 진입
    public void LoadScene(SceneType sceneType)
    {
        // 코루틴이 아직 남아 있으면 
        if(_loadingCoroutine != null)
        {
            // 코루틴 종료 
            StopCoroutine(_loadingCoroutine);
        }

        // 로드할 씬이 등록되어 있지 않으면 
        if(!_scenes.TryGetValue(sceneType, out var scene))
        {
            // 종료 
            Debug.LogError($"SceneType이 없습니다. : {sceneType}");
            return;
        }

        // 로드할 씬이 현재 씬과 같으면 
        if(_currentScene == scene)
        {
            // 종료 -> 상황에 따라 다름. 같아도 종료하지 않는 경우도 있음. 
            return;
        }

        // 씬 로딩 (비동기 처리) 
        _loadingCoroutine = StartCoroutine(Load_SceneProcess(sceneType));
    }

    IEnumerator Load_SceneProcess(SceneType sceneType)
    {
        // 로드할 씬을 가져온다.
        var scene = _scenes[sceneType];

        // 기존 씬이 있으면 씬에 등록된 콜백들을 해제하고 종료한다. 
        _currentScene?.OnSceneExit();

        // 페이드 아웃 
        yield return StartCoroutine(FadeOut());

        // 이전 씬 저장, 로드한 씬을 현재 씬으로 저장 
        _prevScene = _currentScene;
        _currentScene = scene;

        // 씬 비동기 로드 
        var operation = SceneManager.LoadSceneAsync(sceneType.ToString());

        // 씬이 준비되지 않았는데, 전환되는 것을 방지하기 위해 allowSceneActivation을 false로 
        operation.allowSceneActivation = false;

        // 씬 로딩과 별도로 로딩할 것들이 있는지 (몬스터 로드, 이미지, 영상 로드, 친구 데이터 로드 이런 것들) 처리 
        _currentScene.SceneLoading();

        // operation.progress가 0.9가 될때까지만 기다린다. (씬 로딩이 다 될때까지 기다린다)
        while(operation.progress < 0.9f)
        {
            yield return null;
        }

        // 씬 전환을 허가
        operation.allowSceneActivation = true;  

        // 씬 전환 완료를 기다리고 
        while (!operation.isDone)
        {
            yield return null;
        } // 여기서 씬 전환 

        // 한프레임 전환 대기
        // (씬에 있는 오브젝트 - OnEnable / Awake / Start 등 초기화 대기)
        yield return null;

        // 로딩된 씬 진입 콜백함수 실행 
        _currentScene.OnSceneEnter();

        GameManager.Instance.Set_Player();

        GameManager.Instance.PortalInfo = PortalDirection.None;

        SoundManager.Instance.All_Stop_Ambience();

        // 페이드 인
        yield return StartCoroutine(FadeIn());

        // 씬 전환이 끝났으니 로딩 코루틴을 더이상 참조할 수 없게 null로 만들기 
        _loadingCoroutine = null;
    }

    public void End_Game()
    {
        GameOverPanel.SetActive(true);
    }

    private IEnumerator FadeOut()
    {
        _animator.SetTrigger(UI.FADEIN);

        yield return new WaitForSeconds(transitionTime);
    }

    private IEnumerator FadeIn()
    {
        _animator.SetTrigger(UI.FADEOUT);

        yield return new WaitForSeconds(transitionTime);
    }
}
