using UnityEngine;
using HorseFSMState;

public class HorseFSM : MonoBehaviour
{
    public Vector3[] PatrolPositions => _patrolPositions;
    [SerializeField] private Transform[] patrolPoints;
    private Vector3[] _patrolPositions;
    
    private Horse _horse;
    private FSM<FSMState> _fsm;
    
    private void Awake()
    {
        _horse = GetComponent<Horse>();
        
        _fsm = new FSM<FSMState>();
        
        // PatrolPoint의 Transform을 Vector3로 저장
        _patrolPositions = new Vector3[patrolPoints.Length];
        for (int i = 0; i < patrolPoints.Length; i++)
        {
            _patrolPositions[i] = patrolPoints[i].position;
        }
    }
    
    private void Start()
    {
        // 순찰 상태 변경
        _fsm.ChangeState(new HorseFSMState.PatrolState(_horse, this, _fsm));
    }

    private void FixedUpdate()
    {
        _fsm.Update();
    }
    
    public int GetClosestPointIndex()
    {
        // 가까운 순찰 point 검색
        float minDist = float.MaxValue;
        int index = 0;
        for (int i = 0; i < _patrolPositions.Length; i++)
        {
            float dist = Vector3.Distance(_horse.transform.position, _patrolPositions[i]);
            if (dist < minDist)
            {
                minDist = dist;
                index = i;
            }
        }
        return index;
    }
}
