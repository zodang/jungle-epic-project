using UnityEngine;

public class WallColliderMover : MonoBehaviour
{
    private Camera _camera;
    private Vector3 _targetPos;
    [SerializeField] private float pos1609;
    [SerializeField] private float pos1610;

    private void Start()
    {
        _camera = Camera.main;
    }

    private void FixedUpdate()
    {
        transform.position = _camera.aspect <= 1.6f
            ? new Vector3(pos1609, transform.position.y, transform.position.z)
            : new Vector3(pos1610, transform.position.y, transform.position.z);
    }
}
