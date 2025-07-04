using Unity.Cinemachine;
using UnityEngine;

public class CameraFocusZone : MonoBehaviour
{
    public Color triggerAreaGizmoColor = new Color(0.69f, 0f, 0.45f, 0.6f);
    public float triggerRangeX = 1f;
    public float triggerRangeY = 1f;
    public float triggerOffsetX = 0f;
    public float triggerOffsetY = 0f;

    private CinemachineTargetGroup _cinemachineTargetGroup;
    private Transform _target;
    private bool _isIn;
    private bool _prevIsIn;


    private void Awake()
    {
        _cinemachineTargetGroup = FindAnyObjectByType<CinemachineTargetGroup>();
        _target = _cinemachineTargetGroup.transform;

        _isIn = false;
        _prevIsIn = _isIn;
    }

    private void Update()
    {
        float distX = _target.position.x - (transform.position.x + triggerOffsetX);
        float distY = _target.position.y - (transform.position.y + triggerOffsetY);

        bool isIn = distX < triggerRangeX && distX > -triggerRangeX
            && distY < triggerRangeY && distY > -triggerRangeY;

        if (isIn == _prevIsIn) return;
        _prevIsIn = isIn;

        transform.GetChild(0).gameObject.SetActive(isIn);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = triggerAreaGizmoColor;

        Gizmos.DrawCube(new Vector3(
            transform.position.x + triggerOffsetX
            , transform.position.y + triggerOffsetY
            , transform.position.z)
            , new Vector3(
                triggerRangeX * 2
                , triggerRangeY * 2, 0f)
            );
    }
#endif
}
