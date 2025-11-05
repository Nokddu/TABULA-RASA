using UnityEngine;

public class Player_DogState : FormBaseState
{
    protected override CurrentState _myCurrentState => CurrentState.Dog;
    public Player_DogState(StateMachine StateMachine,Player player) : base(StateMachine,player)
    {
    }

    public override void Enter()
    {
        base.Enter();
        //Debug.Log("Now State : DogState");
        FormSO Data = _player.GetFormData(CurrentState.Dog);
        
        if (Data != null)
        {
            _player.Animator.runtimeAnimatorController = Data.FormData.Animator;
            _player.Collider2D.size = Data.FormData.ColliderSize;
            _player.Collider2D.offset = Data.FormData.ColliderOffset;
        }

        _player.UpdateScentTracking();
        _player.PlayerStatusUI.UpdatePortrait(2);
    }

    public override void Exit()
    {
        base.Exit();
        _player.StopSniff();
    }

    public override void Update()
    {
        base.Update();
    }

    protected override void Get_Item()
    {
        base.Get_Item();
        _player.UpdateScentTracking();
    }

    protected override void Interact()
    {
        base.Interact();
        if (_player.IsInteract() || _player.IsCanInventory()) return;
        if (_currentNpcTarget != null)
        {
            _currentNpcTarget?.Dialog(CurrentState.Dog);
            _player.InteractOn(_currentNpcTarget.transform.position,true,true,true);
        }
    }
}

