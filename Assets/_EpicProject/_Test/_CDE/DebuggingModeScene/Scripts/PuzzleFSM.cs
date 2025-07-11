using UnityEngine;

public class PuzzleFSM : MonoBehaviour
{
    private FSM<FSMState> _fsm;
    
    private void Awake()
    {
        _fsm = new FSM<FSMState>();
    }

    public void StartPuzzle()
    {
        _fsm.ChangeState(new LightStepState(this, _fsm));
    }

    private void Update()
    {
        _fsm.Update();
    }
}
