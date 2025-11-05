using UnityEngine;

public class Player_CatState : FormBaseState
{
    protected override CurrentState _myCurrentState => CurrentState.Cat;
    public Player_CatState(StateMachine StateMachine, Player player) : base(StateMachine, player)
    {
        
    }

    public override void Enter()
    {
        base.Enter();
        //Debug.Log("Now State : CatState");

        FormSO Data = _player.GetFormData(CurrentState.Cat);

        if (Data != null)
        {
            _player.Animator.runtimeAnimatorController = Data.FormData.Animator;
            _player.Collider2D.size = Data.FormData.ColliderSize;
            _player.Collider2D.offset = Data.FormData.ColliderOffset;
        }
        _player.PlayerStatusUI.UpdatePortrait(1);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
    }

    protected override void Interact()
    {
        base.Interact();
        if (_player.IsInteract() || _player.IsCanInventory()) return;
        if (_currentNpcTarget != null)
        {
            _currentNpcTarget?.Dialog(CurrentState.Cat);
            _player.InteractOn(_currentNpcTarget.transform.position, true, true, true);
        }
    }
}
