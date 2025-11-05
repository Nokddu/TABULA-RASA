using UnityEngine;

public class Player_InteractState : BaseState
{
    public Player_InteractState(ActionStateMachine StateMachine, Player player) : base(StateMachine)
    {
    }
    public override void Enter()
    {
        base.Enter();

        //Debug.Log("Now State : Interact");
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
    }

    public override void HandleInput()
    {
        base.HandleInput();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
