
public class ActionStateMachine : StateMachine
{
    public Player Player { get; }
    public Player_IdleState IdleState { get; private set; }
    public Player_WalkState WalkState { get; private set; }
    public Player_InteractState InteractState { get; private set; }

    public Player_UIState UIState { get; private set; }
    // 스테이트 머신과 플레이어 간에 낮은 결합 시작할때 State를 GhostState로 Init 해줌//
    public ActionStateMachine(Player player)
    {
        this.Player = player;

        IdleState = new Player_IdleState(this,player);
        WalkState = new Player_WalkState(this,player.Data.PlayerData, player);
        InteractState = new Player_InteractState(this, player);

        UIState = new Player_UIState(this);
    }

    public void Initialize()
    {
        currentState = IdleState;
        currentState.Enter();
    }

    public void InteractOn()
    {
        Change_State(InteractState);
    }

    public void InteractOff()
    {
        Change_State(IdleState);
    }
}
