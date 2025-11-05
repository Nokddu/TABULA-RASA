public interface IState
{
    public void Exit();
    public void Enter();
    public void Update();
    public void HandleInput();
    public void PhysicsUpdate();
}

public abstract class StateMachine
{
    protected IState currentState;
    
    public void Change_State(IState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState?.Enter();
    }

    public void Handle_Input()
    {
        currentState?.HandleInput();
    }

    public void Update()
    {
        currentState?.Update();
    }

    public void Physics_Update()
    {
        currentState?.PhysicsUpdate();
    }
}
