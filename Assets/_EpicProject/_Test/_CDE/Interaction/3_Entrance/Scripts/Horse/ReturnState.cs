using UnityEngine;
using HorseFSMState;

namespace HorseFSMState
{
    public class ReturnState : FSMState
    {
        private Horse _horse;
        private HorseFSM _patrolFsm;
        private FSM<FSMState> _fsm;
        
        private Vector3 _targetPoint;
    
        private bool _waiting;
        private float _waitTimer;
        private float _waitTime = 0.3f;
    
        public ReturnState(Horse horse, HorseFSM patrolFsm, FSM<FSMState> fsm)
        {
            _horse = horse;
            _patrolFsm = patrolFsm;
            _fsm = fsm;
        }
        
        public override void Enter()
        {
            _horse.OnControlEnabled += ChangeToControlState;
            _targetPoint = _patrolFsm.PatrolPositions[_patrolFsm.GetClosestPointIndex()];
        }

        public override void Update()
        {
            if (_waiting)
            {
                // _waitTime 동안 대기
                _waitTimer += Time.deltaTime;
                if (_waitTimer >= _waitTime)
                {
                    _fsm.ChangeState(new PatrolState(_horse, _patrolFsm, _fsm));
                }
                return;
            }

            RigidbodyMove();
        }

        public override void Exit()
        {
            _horse.OnControlEnabled -= ChangeToControlState;
        }
        
        private void ChangeToControlState()
        {
            _fsm.ChangeState(new ControlState(_horse, _patrolFsm, _fsm));
        }
        
        private void RigidbodyMove()
        {
            // Movement2D 사용한 이동
            Vector3 pos = _horse.transform.position;
            Vector3 target = _targetPoint;
            Vector2 dir = (target - pos).normalized;
            
            _horse.SetMoveDirection(dir);

            if (!(Vector3.Distance(_horse.transform.position, _targetPoint) < 1)) return;
        
            // targetPoint에 도달
            _waiting = true;
            _waitTimer = 0f;
            
            _fsm.ChangeState(new PatrolState(_horse, _patrolFsm, _fsm));
        }
    }
}