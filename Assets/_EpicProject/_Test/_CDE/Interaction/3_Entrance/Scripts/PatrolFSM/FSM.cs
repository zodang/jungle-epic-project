public class FSM<T> where T : FSMState
{
    public T CurrentState { get; private set; }

    public void ChangeState(T newState)
    {
        CurrentState?.Exit();
        CurrentState = newState;
        CurrentState?.Enter();
    }

    public void Update()
    {
        CurrentState?.Update();
    }
}