using UnityEngine;

public class Player_IdleState : BaseState
{
    private Player _player;
    public Player_IdleState(ActionStateMachine stateMachine, Player player) : base(stateMachine)
    {
        _player = player;
    }

    public override void Enter()
    {
        //Debug.Log("Now State : IdleState");
    }

    public override void Exit()
    {

    }

    public override void Update()
    {
        base.Update();
        if (_player.MovementInput != Vector2.zero)
        {
            ActionStateMachine FSM = _stateMachine as ActionStateMachine;
            _stateMachine.Change_State(FSM.WalkState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

    }

}
