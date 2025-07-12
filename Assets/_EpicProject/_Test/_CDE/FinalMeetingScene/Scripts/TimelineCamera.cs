using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class TimelineCamera : MonoBehaviour
{
    [SerializeField] List<Transform> targets;

    private int _index = 0;
    private Transform _defaultTarget;

    private bool _isChanging;
    private float _moveSpeed = 5f;
    private float _stopDistance = 0.1f;

    private CinemachineFollow _followCamera;
    private CinemachineGroupFraming _groupCamera;
    private void Awake()
    {
        _defaultTarget = FindAnyObjectByType<CameraFraming>().transform;

        _followCamera = GetComponent<CinemachineFollow>();
        _groupCamera = GetComponent<CinemachineGroupFraming>();
        
        //targets[targets.Count] = _defaultTarget;
    }

    public void ChangeTarget()
    {
        _isChanging = true;
        _followCamera.enabled = false;
        _groupCamera.enabled = false;
    }
    
    void LateUpdate()
    {
        if (!_isChanging) return;
        
        // 목표 위치
        Vector3 targetPos = new Vector3(targets[_index].position.x, 0, -10);

        // 일정 거리 이내면 이동 중단
        if (Vector3.Distance(transform.position, targetPos) < _stopDistance)
        {
            _isChanging = false;
            // _followCamera.enabled = true;
            // _groupCamera.enabled = true;
            _index++;
            return;
        }

        // 타임스케일 영향 없이 Lerp
        transform.position = Vector3.Lerp(
            transform.position,
            targetPos,
            _moveSpeed * Time.unscaledDeltaTime
        ); 
    }
}
