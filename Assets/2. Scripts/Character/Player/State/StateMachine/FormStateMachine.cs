
public class FormStateMachine : StateMachine
{
    public Player Player { get; }
    public Player_GhostState GhostState { get; set; }
    public Player_CatState CatState { get; set; }
    public Player_DogState DogState { get; set; }
    public Player_HumanState HumanState { get; set; }

    public FormStateMachine(Player player)
    {
        this.Player = player;

        GhostState = new Player_GhostState(this,Player);
        CatState = new Player_CatState(this, Player);
        DogState = new Player_DogState(this, Player);
        HumanState = new Player_HumanState(this, Player);
    }

    public void Initialize(CurrentState startState)
    {
        switch (startState)
        {
            case CurrentState.Human:
                currentState = HumanState;
                break;
            case CurrentState.Cat:
                currentState = CatState;
                break;
            case CurrentState.Dog:
                currentState = DogState;
                break;
            default:
                currentState = GhostState;
                break;
        }
        currentState.Enter();
    }
    public CurrentState GetCurrentStateEnum()
    {
        if (currentState is Player_HumanState) return CurrentState.Human;
        if (currentState is Player_CatState) return CurrentState.Cat;
        if (currentState is Player_DogState) return CurrentState.Dog;

        return CurrentState.Ghost;
    }
}
