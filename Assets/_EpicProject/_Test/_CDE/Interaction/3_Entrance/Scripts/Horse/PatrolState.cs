using UnityEngine;
using HorseFSMState;

namespace HorseFSMState
{
    public class PatrolState : FSMState
    {
        private Horse _horse;
        private HorseFSM _patrolFsm;
        private FSM<FSMState> _fsm;
    
        private int _currentPoint;
    
        public PatrolState(Horse horse, HorseFSM patrolFsm, FSM<FSMState> fsm)
        {
            _horse = horse;
            _patrolFsm = patrolFsm;
            _fsm = fsm;
        }

        public override void Enter()
        {
            _currentPoint = _patrolFsm.GetClosestPointIndex();
            _horse.OnControlEnabled += ChangeToControlState;
        }

        public override void Update()
        {
            if (_patrolFsm.PatrolPositions == null || _patrolFsm.PatrolPositions.Length == 0) return;

            Vector3 pos = _horse.transform.position;
            Vector3 target = _patrolFsm.PatrolPositions[_currentPoint];
            Vector2 dir = (target - pos).normalized;
        
            // 목적지로 이동
            _horse.SetMoveDirection(dir);

            // 목적지 변경
            if (Vector3.Distance(_horse.transform.position, _patrolFsm.PatrolPositions[_currentPoint]) < 0.1f)
            {
                _currentPoint = (_currentPoint + 1) % _patrolFsm.PatrolPositions.Length;
            }
        }

        public override void Exit()
        {
            _horse.OnControlEnabled -= ChangeToControlState;
        }

        private void ChangeToControlState()
        {
            _fsm.ChangeState(new ControlState(_horse, _patrolFsm, _fsm));
        }
    }

}
