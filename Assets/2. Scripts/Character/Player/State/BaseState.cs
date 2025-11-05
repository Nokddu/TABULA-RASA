using UnityEngine;

public abstract class BaseState : IState
{
    protected StateMachine _stateMachine;
    public BaseState(StateMachine StateMachine)
    {
        this._stateMachine = StateMachine;
    }

    public virtual void Enter()
    {

    }
    public virtual void Update()
    {

    }
    public virtual void Exit()
    {

    }

    public virtual void HandleInput()
    {

    }

    public virtual void PhysicsUpdate()
    {
    }
}

public abstract class FormBaseState : BaseState
{
    private Possessable _currentTradeTarget;
    protected Player _player;
    protected Npc _currentNpcTarget;
    protected SavePoint _currentsavePoint;

    protected FormBaseState(StateMachine StateMachine, Player player) : base(StateMachine)
    {
        _player = player;
    }
    protected void ShowInventory()
    {
        if (!_player.IsCanInventory())
        {
            SoundManager.Instance.Play_Sfx(SFX.Inventory);

            if (_currentTradeTarget != null)
            {
                InventoryManager.Instance.ShowTradeInventories(_myCurrentState, _currentTradeTarget.GetFormState());
            }
            else
            {
                InventoryManager.Instance.ShowInventory(_myCurrentState);
            }
        }
    }
    protected virtual void Get_Item()
    {
        Vector2 mousePos = _player.MainCam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 playerPos = _player.transform.position;

        Vector2 rayDirection = (mousePos - playerPos).normalized;

        LayerMask mask = LayerMask.GetMask("Item");
        RaycastHit2D hit = Physics2D.Raycast(playerPos, rayDirection, _player.rayDistance, mask);

        if (hit.collider != null)
        {
            SoundManager.Instance.Play_Sfx(SFX.Gain);

            FieldItem fieldItem = hit.collider.GetComponent<FieldItem>();

            bool success = InventoryManager.Instance.PickUpItem(fieldItem, (int)_myCurrentState);
        }
    }
    public override void Update()
    {
        base.Update();
        CheckForTradeableObject();

        if (_player.Sanity > 0)
        {
            _player.Sanity += _player.RecoverySanity * Time.deltaTime;
        }
    }
    public override void Enter()
    {
        base.Enter();

        _player.Input.OnInventoryEvent += ShowInventory;
        _player.Input.OnItemGetEvent += Get_Item;
        _player.Input.OnInteractEvent += Interact;
        _player.Input.OnPossessEvent += ExitPossession;
    }
    public override void Exit()
    {
        base.Exit();
        _player.Input.OnInventoryEvent -= ShowInventory;
        _player.Input.OnItemGetEvent -= Get_Item;
        _player.Input.OnInteractEvent -= Interact;
        _player.Input.OnPossessEvent -= ExitPossession;
    }
    protected abstract CurrentState _myCurrentState { get; }
    protected void CheckForTradeableObject()
    {
        Possessable oldTradeTarget = _currentTradeTarget;
        Npc oldNpcTarget = _currentNpcTarget;
        _player.TargetNpc = _currentNpcTarget;
        SavePoint oldSavePoint = _currentsavePoint;

        _currentTradeTarget = null;
        _currentNpcTarget = null;
        _currentsavePoint = null;

        Vector2 mousePos = _player.MainCam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 playerPos = _player.transform.position;
        Vector2 rayDirection = (mousePos - playerPos).normalized;

        LayerMask combinedMask1 = LayerMask.GetMask("Possessable");
        LayerMask combinedMask2 = LayerMask.GetMask("Interact");
        RaycastHit2D hit1 = Physics2D.Raycast(playerPos, rayDirection, _player.rayDistance, combinedMask1);
        RaycastHit2D hit2 = Physics2D.Raycast(playerPos, rayDirection, _player.rayDistance, combinedMask2);

        if (hit1.collider != null)
        {
            if (hit1.collider.TryGetComponent<Possessable>(out Possessable potentialTarget))
            {
                if (potentialTarget.GetFormState() != _myCurrentState)
                {
                    _currentTradeTarget = potentialTarget;
                }
            }
        }

        if (hit2.collider != null)
        {
            if (hit2.collider.TryGetComponent<Npc>(out Npc potentialNpc))
            {
                _currentNpcTarget = potentialNpc;
            }
        }

        if (oldTradeTarget != _currentTradeTarget)
        {
            if (oldTradeTarget != null)
            {
                oldTradeTarget.GetComponentInChildren<SpriteOutline>().color = Color.white;
            }
            if (_currentTradeTarget != null)
            {
                _currentTradeTarget.GetComponentInChildren<SpriteOutline>().color = Color.green;
            }
        }

        if (oldNpcTarget != _currentNpcTarget)
        {
            if (oldNpcTarget != null)
            {
                oldNpcTarget.GetComponentInChildren<SpriteOutline>().outlineSize = 0;
            }
            if (_currentNpcTarget != null)
            {
                _currentNpcTarget.GetComponentInChildren<SpriteOutline>().outlineSize = 2.5f;
            }
        }
    }

    protected void ExitPossession()
    {
        if (_player.IsInteract()) return;
        
        _player.OnPossessEnd();

        FormStateMachine FSM = _stateMachine as FormStateMachine;

        _stateMachine.Change_State(FSM.GhostState);

        InventoryManager.Instance.HideAllInventories();

        _player.PlayerStatusUI.UpdatePortrait(0);
    }
    protected virtual void Interact()
    {
    }
}