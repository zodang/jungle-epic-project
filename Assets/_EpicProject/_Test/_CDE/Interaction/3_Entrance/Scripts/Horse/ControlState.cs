namespace HorseFSMState
{
    public class ControlState : FSMState
    {
        private Horse _horse;
        private HorseFSM _patrolFsm;
        private FSM<FSMState> _fsm;
    
        public ControlState(Horse horse, HorseFSM patrolFsm, FSM<FSMState> fsm)
        {
            _horse = horse;
            _patrolFsm = patrolFsm;
            _fsm = fsm;
        }
        
        public override void Enter()
        {
            _horse.OnControlDisabled += ChangeToReturnState;
        }

        public override void Exit()
        {
            _horse.OnControlDisabled -= ChangeToReturnState;
        }
    
        private void ChangeToReturnState()
        {
            _fsm.ChangeState(new ReturnState(_horse, _patrolFsm, _fsm));
        }
    }
}