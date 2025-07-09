using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class CameraFraming : MonoBehaviour
{
    private Transform _backgroundObject;
    private Camera _mainCamera;
    private CinemachineTargetGroup _cinemachineTargetGroup;

    private Vector3 _originBackgroundScale;
    private float _originCameraSize;

    private void Start()
    {
        TryGetComponent<CinemachineTargetGroup>(out _cinemachineTargetGroup);
        _mainCamera = Camera.main;
        if(!_mainCamera.IsUnityNull())
        {
            _backgroundObject = _mainCamera.transform.Find("BackgroundImg");
            if(!_backgroundObject.IsUnityNull())
            {
                _originCameraSize = _mainCamera.orthographicSize;
                _originBackgroundScale = _backgroundObject.localScale;
            }
        }

        // 플레이어 추가
        AddTarget(StageBaseManager.Instance.PlayerManager.transform);
    }

    private void FixedUpdate()
    {
        if (_backgroundObject.IsUnityNull()) return;

        _backgroundObject.localScale = (_mainCamera.orthographicSize * _originBackgroundScale) / _originCameraSize;
    }

    public void AddTarget(Transform target)
    {
        if (_cinemachineTargetGroup.IsUnityNull()) return;

        if(_cinemachineTargetGroup.FindMember(target) < 0)
        {
            _cinemachineTargetGroup.AddMember(target, 1f, 1f);
        }
    }

    public void RemoveTarget(Transform target)
    {
        if (_cinemachineTargetGroup.IsUnityNull()) return;

        _cinemachineTargetGroup.RemoveMember(target);
    }
    
    public void SetTarget(Transform newTarget)
    {
        if (_cinemachineTargetGroup.IsUnityNull()) return;

        // 기존 멤버 모두 제거
        int count = _cinemachineTargetGroup.Targets.Count;
        for (int i = count - 1; i >= 0; i--)
        {
            _cinemachineTargetGroup.Targets.RemoveAt(i);
        }

        // 새로운 멤버 추가
        _cinemachineTargetGroup.AddMember(newTarget, 1, 1);
    }
}


