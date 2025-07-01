using UnityEngine;

public class PatrolFSM : MonoBehaviour
{
    [SerializeField] private Transform[] patrolPoints;
    
    private Vector3[] _patrolPositions;
    public Vector3[] PatrolPositions => _patrolPositions;
    
    private CameraFraming _cameraFraming;
    private EntrancePatrolGuard _guard;
    
    private FSM<FSMState> _fsm;
    
    private void Awake()
    {
        _cameraFraming = FindAnyObjectByType<CameraFraming>();
        _guard = GetComponent<EntrancePatrolGuard>();
        
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
        _fsm.ChangeState(new PatrolState(_guard, this, _fsm));
    }

    private void Update()
    {
        _fsm.Update();
    }
    
    public int GetClosestPointIndex()
    {
        float minDist = float.MaxValue;
        int index = 0;
        for (int i = 0; i < _patrolPositions.Length; i++)
        {
            float dist = Vector3.Distance(_guard.transform.position, _patrolPositions[i]);
            if (dist < minDist)
            {
                minDist = dist;
                index = i;
            }
        }
        return index;
    }


    public void SetFocus()
    {
        _cameraFraming.AddTarget(transform);
        _cameraFraming.RemoveTarget(StageBaseManager.Instance.PlayerManager.transform);
    }

    public void UnsetFocus()
    {
        _cameraFraming.RemoveTarget(transform);
        _cameraFraming.AddTarget(StageBaseManager.Instance.PlayerManager.transform);
    }
}
