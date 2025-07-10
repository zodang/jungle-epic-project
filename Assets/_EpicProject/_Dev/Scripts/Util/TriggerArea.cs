using UnityEngine;
using UnityEngine.Events;

public class TriggerArea : MonoBehaviour
{
    public Color triggerAreaGizmoColor = new Color(0f, 0.7f, 0f, 0.2f);
    public float triggerRangeX = 1f;
    public float triggerRangeY = 1f;

    public Transform target;
    public bool OnlyOnce = true;

    public UnityEvent OnTrigger;
    bool _isTriggered;
    bool _isInRange;

    void Start()
    {
        _isTriggered = false;
        _isInRange = false;
        target = StageBaseManager.Instance.PlayerManager.transform;
    }

    void Update()
    {
        if (target != null)
        {
            if (_isTriggered && OnlyOnce) return;

            float distX = target.position.x - transform.position.x;
            float distY = target.position.y - transform.position.y;

            _isInRange = distX < triggerRangeX && distX > -triggerRangeX
                && distY < triggerRangeY && distY > -triggerRangeY;

            if(_isInRange && !_isTriggered)
            {
                _isTriggered = true;
                Debug.Log("Triggered: " + transform.name);
                OnTrigger?.Invoke();
            }
            else if(!_isInRange && _isTriggered)
            {
                _isTriggered = false;
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = triggerAreaGizmoColor;
        Gizmos.DrawCube(transform.position, new Vector3(triggerRangeX * 2, triggerRangeY * 2, 0f));
    }
#endif
}
