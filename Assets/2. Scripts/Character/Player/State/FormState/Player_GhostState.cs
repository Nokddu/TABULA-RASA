using UnityEngine;

public class Player_GhostState : BaseState
{
    private Player _player;
    private Possessable _currentTarget;
    private Npc _currentNpc;

    private Camera cam;
    public Player_GhostState(FormStateMachine StateMachine, Player player) : base(StateMachine)
    {
        this._player = player;
    }

    // TryPossess 키 Q에 바인딩
    public override void Enter()
    {
        base.Enter();
        cam = _player.MainCam;
        _player.Collider2D.isTrigger = true;
        _player.Input.OnPossessEvent += TryPossess;
        _player.Input.OnInteractEvent += Interact;
        _player.Input.OnItemGetEvent += TryGetItemAsGhost;
        //Debug.Log("Now State : GhostState");
        FormSO Data = _player.GetFormData(CurrentState.Ghost);

        if (Data != null)
        {
            _player.Animator.runtimeAnimatorController = Data.FormData.Animator;
            _player.Collider2D.size = Data.FormData.ColliderSize;
            _player.Collider2D.offset = Data.FormData.ColliderOffset;
        }
    }
    public override void Exit()
    {
        base.Exit();
        _player.Collider2D.isTrigger = false;
        _player.Input.OnPossessEvent -= TryPossess;
        _player.Input.OnInteractEvent -= Interact;
        _player.Input.OnItemGetEvent -= TryGetItemAsGhost;
    }
    public override void Update()
    {
        base.Update();
        CheckForPossessableObject();

        if (_player.Sanity > 0)
        {
            _player.Sanity -= _player.DecaySanity * Time.deltaTime;
        }
    }

    public override void HandleInput()
    {
        base.HandleInput();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    // Ray 활용해서 계속해서 빙의 가능한 물체가 플레이어와 마우스의 일정한 rayDistance 사이에 있는지 확인하는 메서드
    private void CheckForPossessableObject()
    {
        Possessable oldTarget = _currentTarget;
        Npc oldNpcTarget = _currentNpc;

        _currentTarget = null;
        _currentNpc = null;

        Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 playerPos = _player.RayPoint.position;

        Vector2 rayDirection = (mousePos - playerPos).normalized;

        LayerMask mask = LayerMask.GetMask("Possessable","Interact");
        RaycastHit2D hit = Physics2D.Raycast(playerPos, rayDirection, _player.rayDistance, mask);

        if (hit.collider != null)
        {
            if (hit.collider.TryGetComponent<Possessable>(out Possessable potentialTarget))
            {
                _currentTarget = potentialTarget;
            }

            // Npc인지 확인
            if (hit.collider.TryGetComponent<Npc>(out Npc potentialNpc))
            {
                _currentNpc = potentialNpc;
            }
        }

        if (oldTarget != _currentTarget)
        {
            if (oldTarget != null)
            {
                oldTarget.GetComponentInChildren<SpriteOutline>().color = Color.white;
                oldTarget.OffText();
            }
            if (_currentTarget != null)
            {
                _currentTarget.OnText();
                _currentTarget.GetComponentInChildren<SpriteOutline>().color = Color.red;
            }
        }

        if (oldNpcTarget != _currentNpc)
        {
            if (oldNpcTarget != null)
            {
                oldNpcTarget.GetComponentInChildren<SpriteOutline>().outlineSize = 0;
            }
            if (_currentNpc != null)
            {
                _currentNpc.GetComponentInChildren<SpriteOutline>().outlineSize = 1;
            }
        }
    }
    //빙의 가능한 물체인지 확인하고 빙의하는 메서드
    private void TryPossess()
    {
        if (_player.IsInteract()) return;
        //Debug.Log("Can Possess");
        if (_currentTarget == null) return;

        _player.OnPossessStart(_currentTarget);
        FormStateMachine FSM = _stateMachine as FormStateMachine;
        if (_currentTarget.Pooltag == "Human")
        {
            _stateMachine.Change_State(FSM.HumanState);
        }
        else if (_currentTarget.Pooltag == "Dog")
        {
            _stateMachine.Change_State(FSM.DogState);
        }
        else if (_currentTarget.Pooltag == "Cat")
        {
            _stateMachine.Change_State(FSM.CatState);
        }
        _player.Animator.SetTrigger("Possess_Start");
        _currentTarget = null;
    }

    private void Interact()
    {
        if (_player.IsInteract() || _player.IsCanInventory()) return;
        if(SaveManager.Instance.UserData.Level < 1) 
        {
            return;
        }

        if (_currentNpc != null)
        {
            _currentNpc?.Dialog(CurrentState.Ghost);
            _player._actionStateMachine.InteractOn();
        }
    }
    private void TryGetItemAsGhost()
    {
        NotificationManager.Instance.ShowGhostMessage();
    }
}
