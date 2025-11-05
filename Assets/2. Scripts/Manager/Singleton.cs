using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Component
{
    private static T _instance;
    private static bool isShuttingDown = false; // 앱 종료 시 인스턴스 재생성 방지용 플래그
    private static readonly object _lock = new object(); // 스레드 안전성(Thread-Safety)을 위한 Lock 객체

    public static T Instance
    {
        get
        {
            // 애플리케이션이 종료된 상태에서 Instance를 호출하면 null 반환
            if (isShuttingDown)
            {
                Debug.LogWarning($"[Singleton] Instance '{typeof(T)}' already destroyed on application quit. Won't create again - returning null.");
                return null;
            }

            // 스레드 동기화 처리
            lock (_lock)
            {
                if (_instance == null)
                {
                    // 씬에서 먼저 찾아본다.
                    _instance = FindObjectOfType<T>();

                    // 씬에 없다면 새로 생성한다.
                    if (_instance == null)
                    {
                        GameObject go = new GameObject(typeof(T).ToString() + " [Singleton]");
                        _instance = go.AddComponent<T>();
                    }
                }
                return _instance;
            }
        }
    }

    protected virtual void Awake()
    {
        // 싱글턴 인스턴스가 2개 이상 생성되는 것을 방지
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (_instance != this)
        {
            Debug.LogWarning($"[Singleton] Instance of '{typeof(T)}' already exists. Destroying new one.");
            Destroy(this.gameObject);
        }
    }

    protected virtual void OnApplicationQuit()
    {
        isShuttingDown = true;
    }

    protected virtual void OnDestroy()
    {
        // 씬 전환 등으로 인해 파괴될 경우, isShuttingDown 플래그는 false로 유지
        if (_instance == this)
        {
            // isShuttingDown이 true가 아니라면, static instance를 null로 만들어
            // 다음에 다시 생성될 수 있도록 함.
            if (!isShuttingDown)
            {
                _instance = null;
            }
        }
    }
}