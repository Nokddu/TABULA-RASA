using UnityEngine;

public class Player_HumanState : FormBaseState
{

    protected override CurrentState _myCurrentState => CurrentState.Human;

    public Player_HumanState(StateMachine StateMachine,Player player) : base(StateMachine, player)
    {

    }

    public override void Enter()
    {
        base.Enter();
        //Debug.Log("Now State : Human");

        FormSO humanData = _player.GetFormData(CurrentState.Human);

        if (humanData != null)
        {
            _player.Animator.runtimeAnimatorController = humanData.FormData.Animator;
            _player.Collider2D.size = humanData.FormData.ColliderSize;
            _player.Collider2D.offset = humanData.FormData.ColliderOffset;
        }

        _player.PlayerStatusUI.UpdatePortrait(3);
    }

    public override void Exit()
    {
        base.Exit();

    }

    protected override void Interact()
    {
        base.Interact();
        if (_player.IsInteract() || _player.IsCanInventory()) return;
        if (_currentNpcTarget != null)
        {
            _currentNpcTarget?.Dialog(CurrentState.Human);
            _player.InteractOn(_currentNpcTarget.transform.position, true, true, true);
        }
    }
}