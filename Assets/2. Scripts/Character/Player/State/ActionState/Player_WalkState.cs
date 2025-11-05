using UnityEngine;

public class Player_WalkState : BaseState
{
    private Player _player;
    private PlayerData _ghostData;
    public Player_WalkState(ActionStateMachine stateMachine, PlayerData ghostData,Player player) : base(stateMachine)
    {
        _player= player;
        _ghostData=ghostData;
    }

    public override void Enter()
    {
        if(SaveManager.Instance.UserData.Players.PoolTag == "Human")
        {
            SoundManager.Instance.Play_Loop_Sfx(SFX.Walk);
        }
        else if(SaveManager.Instance.UserData.Players.PoolTag != null)
        {
            SoundManager.Instance.Play_Loop_Sfx(SFX.AnimalWalk);
        }

        //Debug.Log("Now State : WalkState");
    }
    public override void Exit()
    {
        base.Exit();

        SoundManager.Instance.Stop_Loop_Sfx();
        _player.RigidBody.velocity = Vector2.zero;
    }

    public override void Update()
    {
        base.Update();

        if (_player.RigidBody.velocity == Vector2.zero)
        {
            SoundManager.Instance.Stop_Loop_Sfx();
            ActionStateMachine FSM = _stateMachine as ActionStateMachine;
            _stateMachine.Change_State(FSM.IdleState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        Vector2 moveDirection = _player.MovementInput;

        float targetSpeed = _ghostData.BaseSpeed;

        _player.RigidBody.velocity = moveDirection * targetSpeed;
    }
}
