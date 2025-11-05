using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum CurrentState
{
    Ghost,
    Human,
    Cat,
    Dog,
    All,
}

public class Player : MonoBehaviour
{
    public Transform RayPoint;

    public UserData UserData => SaveManager.Instance.UserData;
    [field: SerializeField] public PlayerSO Data { get; private set; }
    [field: Header("Animations")]
    [field: SerializeField] public PlayerAnimationData AnimationData { get; set; }

    [SerializeField] private List<FormSO> formDatas;
    private Dictionary<CurrentState, FormSO> formDataDict;

    public Animator Animator { get; set; }
    public PlayerController Input { get; private set; }
    public Rigidbody2D RigidBody { get; set; }
    public BoxCollider2D Collider2D;

    public ActionStateMachine _actionStateMachine;

    private FormStateMachine _formStateMachine;

    public Camera MainCam;
    public Vector2 MovementInput;

    public PlayerStatusUI PlayerStatusUI; // 초상화 및 San Bar
    public SoulTierUI soulTierUI;

    private Coroutine trackingCoroutine;
    #region Bool값 관련

    private bool _isInteract = false;

    private bool _isOpenInventory = false;

    private bool _isTalking = false;

    private bool isDie = false;

    private bool _isPosseess =false;

    #endregion

    public GameObject ArrowPivotObj;
    #region 정신력 관련 변수
    public float MaxSanity = 100f; // Max 정신력 수치
    public float DecaySanity = 20f; // 초당 감소하는 정신력 수치
    public float RecoverySanity = 15f;
    private float _sanity; // 프로퍼티 쓰려고
    public float MinimumSanity;
    #endregion

    private PostProcessVolume processVolume; // 포스트 프로세싱 효과
    private Vignette _vignette;

    public Npc TargetNpc;

    public Action LevelUI;

    public float rayDistance {get;} = 3f;
    public float Sanity
    {
        get { return _sanity; }
        set
        {
            _sanity = Mathf.Clamp(value, MinimumSanity, MaxSanity);

            if (PlayerStatusUI != null)
            {
                PlayerStatusUI.UpdateSanityBar(_sanity, MaxSanity);
            }

            if (_vignette != null)
            {
                // Sanity가 33 이하면 효과를 강하게, 33 초과면 효과를 없앱니다.
                if (_sanity <= 40f)
                {
                    // 33에서 0 사이의 값을 0에서 0.5 사이의 값으로 변환하여 부드럽게 어두워지게 함
                    float newIntensity = Mathf.Lerp(0.6f, 0f, _sanity / 40f);
                    _vignette.intensity.value = newIntensity;
                }
                else
                {
                    // 정신력이 33보다 높으면 효과를 완전히 끕니다.
                    _vignette.intensity.value = 0f;
                }
            }

            if (_sanity <= 0 && isDie == false)
            {
                isDie = true;
                PlayerDie();

                _actionStateMachine.InteractOn();
            }
        }
    }

    // 플레이어의 기본적인 정보들을 담고있다.
    private void Awake()
    {
        AnimationData.Initailize();
        Animator = GetComponentInChildren<Animator>();
        Input = GetComponent<PlayerController>();
        RigidBody = GetComponent<Rigidbody2D>();
        Collider2D = GetComponent<BoxCollider2D>();

        MainCam = Camera.main;

        _actionStateMachine = new ActionStateMachine(this);
        _formStateMachine = new FormStateMachine(this);

        formDataDict = new Dictionary<CurrentState, FormSO>();
        foreach (var data in formDatas)
        {
            formDataDict[data.FormData.FormType] = data;
        }

        processVolume = FindObjectOfType<PostProcessVolume>(); // PostProcess Find

        Input.OnMenuEvent += UIManager.Instance.HandleEscapeInput;
        LevelUI += LevelUIUpdate;
    }

    private void Start()
    {
        GameManager.Instance.Register_Player(this);

        Sanity = MaxSanity; //주의동 추가 수정 사항 : 정신력 저장하기 위함.
        _actionStateMachine.Initialize();

        if (GameManager.Instance.HasSavedState)
        {
            GameManager.Instance.Clear_Saved_State();
        }
        else
        {
            _formStateMachine.Initialize(CurrentState.Ghost);
        }

        processVolume.profile.TryGetSettings(out _vignette);

        LevelUI.Invoke();
    }

    private void Update()
    {
        MovementInput = Input.PlayerActions.Move.ReadValue<Vector2>();

        if (!_isInteract)
        {
            _actionStateMachine.Handle_Input();
            _actionStateMachine.Update();
            _formStateMachine.Update();
            HandleAnimations();
        }
    }

    private void LateUpdate()
    {
        MainCam.transform.position = new Vector3(transform.position.x, transform.position.y + 1, -10);
    }

    private void FixedUpdate()
    {
        _actionStateMachine.Physics_Update();
    }

    private void OnDestroy()
    {
        Input.OnMenuEvent -= UIManager.Instance.HandleEscapeInput;
    }

    private void HandleAnimations()
    {
        Vector2 moveInput = RigidBody.velocity;

        if (moveInput != Vector2.zero)
        {
            Animator.SetFloat(AnimationData.LastMoveXParameterHash, moveInput.x);
            Animator.SetFloat(AnimationData.LastMoveYParameterHash, moveInput.y);
        }

        Animator.SetFloat(AnimationData.MoveXParameterHash, moveInput.x);
        Animator.SetFloat(AnimationData.MoveYParameterHash, moveInput.y);

        Animator.SetFloat(AnimationData.MoveSpeedParameterHash, moveInput.magnitude);
    }

    public void Start_Possess()
    {
        Data.PlayerData.PoolTag = SaveManager.Instance.UserData.Players.PoolTag;

        switch (Data.PlayerData.PoolTag)
        {
            case "Human":
                _formStateMachine.Change_State(_formStateMachine.HumanState);
                break;
            case "Dog":
                _formStateMachine.Change_State(_formStateMachine.DogState);
                break;
            case "Cat":
                _formStateMachine.Change_State(_formStateMachine.CatState);
                break;
        }

        foreach (var save in SaveManager.Instance.UserData.PlayerVessel)
        {
            if (Data.PlayerData.PoolTag == save.Tag && save.AreaIndex == -1)
            {
                ObjectPool.Instance.Return_To_Pool(save.Tag, ObjectPool.Instance.VesselChildrenList[save.PrefabsIndex]);
            }
        }
    }

    public void OnPossessStart(Possessable target)
    {
        _isPosseess = true;
        if (_isPosseess)
        {
            StartCoroutine(PossessStart(target));
        }
    }

    public IEnumerator PossessStart(Possessable target)
    {
        _isPosseess = false;
        SFX sfxToPlay = target.Pooltag switch
        {
            "Cat" => SFX.PossessCat,
            "Dog" => SFX.PossessDog,
            "Human" => SFX.Soul,
            _ => SFX.Soul
        };

        SoundManager.Instance.Play_Sfx(sfxToPlay);

        this.transform.position = new Vector2(target.transform.position.x, target.transform.position.y - 1f);
        Data.PlayerData.PoolTag = target.Pooltag;

        foreach (var save in SaveManager.Instance.UserData.PlayerVessel)
        {
            if (Data.PlayerData.PoolTag == save.Tag && save.AreaIndex == SaveManager.Instance.UserData.SceneNumber)
            {
                save.AreaIndex = -1;
            }
        }

        SaveManager.Instance.UserData.Players.PoolTag = target.Pooltag;
        ObjectPool.Instance.Return_To_Pool(target.Pooltag, target.gameObject);

        InteractOn(Vector2.right, true, true, true);

        yield return new WaitForSeconds(2f);

        InteractOff();
        _isPosseess = true;
    }

    public void OnPossessEnd()
    {
        SoundManager.Instance.Play_Sfx(SFX.SoulCancel);

        foreach (var save in SaveManager.Instance.UserData.PlayerVessel)
        {
            if (Data.PlayerData.PoolTag == save.Tag && save.AreaIndex == -1)
            {
                save.AreaIndex = SaveManager.Instance.UserData.SceneNumber;

                save.PosX = RayPoint.position.x;
                save.PosY = RayPoint.position.y;
            }
        }

        ObjectPool.Instance.Off_Pool_Vessel(Data.PlayerData.PoolTag, RayPoint.position);
        Data.PlayerData.PoolTag = null;

        SaveManager.Instance.UserData.Players.PoolTag = null;
    }

    public FormSO GetFormData(CurrentState state)
    {
        formDataDict.TryGetValue(state, out FormSO data);
        return data;
    }

    // NPC 한테 말 걸었을 때 NPC 방향으로 플레이어가 바라보고 다른 상호작용 안되게
    public void InteractOn(Vector2 direction,bool interact, bool inventory, bool talk)
    {
        _isInteract = interact;
        _isOpenInventory = inventory;
        _isTalking = talk;
        MovementInput = Vector2.zero;
        Vector3 dir = new Vector3(direction.x, direction.y, 0) - transform.position;
        Animator.SetFloat(AnimationData.LastMoveXParameterHash, dir.x);
        Animator.SetFloat(AnimationData.LastMoveYParameterHash, dir.y);
        Animator.SetFloat(AnimationData.MoveXParameterHash, 0);
        Animator.SetFloat(AnimationData.MoveYParameterHash, 0);

        Animator.SetFloat(AnimationData.MoveSpeedParameterHash, 0);
        _actionStateMachine.InteractOn();
    }

    public bool IsInteract()
    {
        return _isInteract;
    }

    public bool IsCanInventory()
    {
        return _isOpenInventory;
    }

    public bool IsTalking()
    {
        return _isTalking;
    }

    public IEnumerator StopMove()
    {
        yield return new WaitForSeconds(0.1f);
        _isInteract = false;
        _isOpenInventory = false;
        _isTalking = false;
        _actionStateMachine.InteractOff();
        //Debug.Log("InteractOff");
    }


    public void InteractOff()
    {
        StartCoroutine(StopMove());
    }

    public void StartSniff(int scentId)
    {
        if (trackingCoroutine != null)
        {
            StopCoroutine(trackingCoroutine);
        }
        // 새로운 추적 코루틴을 시작하고, 그 정보를 변수에 저장한다.
        trackingCoroutine = StartCoroutine(TrackScentLocation(scentId));
    }

    public void StopSniff()
    {
        if (trackingCoroutine != null)
        {
            StopCoroutine(trackingCoroutine);
            trackingCoroutine = null;
            
        }
        ArrowPivotObj.SetActive(false);
    }


    IEnumerator TrackScentLocation(int scentId)
    {
        ArrowPivotObj.SetActive(true);
        while (true)
        {
            List<ScentTarget> allTargets = ScentTargetManager.AllTargets;
            List<ScentTarget> foundTargets = new List<ScentTarget>();

            foreach (ScentTarget target in allTargets)
            {
                if (target.ScentId == scentId)
                {
                    foundTargets.Add(target);
                }
            }

            if (foundTargets.Count > 0)
            {
                ScentTarget closeTarget = FindCloseTarget(foundTargets);
                if (closeTarget != null)
                {
                    Vector2 direction = closeTarget.transform.position - this.transform.position;

                    float angle = Mathf.Atan2(-direction.x, direction.y) * Mathf.Rad2Deg;

                    ArrowPivotObj.transform.rotation = Quaternion.Euler(0f, 0f, angle);
                }
            }
            else
            {
                ArrowPivotObj.SetActive(false);
            }

            yield return new WaitForSeconds(0.05f);
        }
    }

    private ScentTarget FindCloseTarget(List<ScentTarget> targets)
    {
        ScentTarget closeTarget = null;
        float minDistance = Mathf.Infinity;
        Vector2 currentPosition = this.transform.position;

        foreach (ScentTarget target in targets)
        {
            float distance = Vector3.Distance(target.transform.position, currentPosition);
            if (distance < minDistance)
            {
                closeTarget = target;
                minDistance = distance;
            }
        }

        return closeTarget;
    }

    public void UpdateScentTracking()
    {
        List<int> ScentItems = new List<int>();
        
        foreach (ItemInfo Item in UserData.PlayerItemData.DogInventory)
        {
            ScentItems.Add(Item.Id);
        }

        if (ScentItems.Count <= 0)
        {
            StopSniff();
            UserData.Players.CurrentTrackedScentId = 0;
            return;
        }

        int newTargetId = 0;

        int currentId = UserData.Players.CurrentTrackedScentId;

        if (currentId != 0 && ScentItems.Contains(currentId) && ScentTargetManager.DoTargetsExist(currentId))
        {
            newTargetId = currentId;
        }
        else
        {
            foreach (int itemId in ScentItems)
            {
                if (ScentTargetManager.DoTargetsExist(itemId))
                {
                    newTargetId = itemId; 
                    break; 
                }
            }
        }

        if (newTargetId != 0)
        {
            UserData.Players.CurrentTrackedScentId = newTargetId;
            StartSniff(newTargetId);
        }
        else
        {
            StopSniff();
            UserData.Players.CurrentTrackedScentId = 0;
        }
    }

    public bool CardCheck()
    {
        bool ret = false;

        if (TargetNpc.gameObject.TryGetComponent<CheckCardKey>(out CheckCardKey Check))
        {
            ret = Check.IsOk();
        }

        return ret;
    }

    public void PlayerDie()
    {
        InteractOn(Vector2.down,true,true,true);
        Animator.SetBool(AnimationData.Die, true);
        StartCoroutine(Main());
    }

    private IEnumerator Main()
    {
        yield return new WaitForSeconds(2f);

        SceneLoadManager.Instance.End_Game();

        SaveManager.Instance.Load_Data();
    }

    private void LevelUIUpdate()
    {
        soulTierUI.UpdateTierDisplay(UserData.Level);
    }
}