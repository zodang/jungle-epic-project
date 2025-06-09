using DG.Tweening.Core.Easing;
using UnityEngine;
using UnityEngine.Events;

public class TriggerArea : MonoBehaviour
{
    public Color triggerAreaGizmoColor = new Color(0f, 0.7f, 0f, 0.2f);
    public float triggerRangeX = 1f;
    public float triggerRangeY = 1f;

    public Transform target;

    public UnityEvent OnTrigger;
    bool _isTriggered;

    void Start()
    {
        //target = FindAnyObjectByType<PlayerFeature>().transform;
        _isTriggered = false;
    }

    void Update()
    {
        if (target != null)
        {
            if (_isTriggered) return;

            float distX = target.position.x - transform.position.x;
            float distY = target.position.y - transform.position.y;

            if (distX < triggerRangeX && distX > -triggerRangeX
                && distY < triggerRangeY && distY > -triggerRangeY
                )
            {
                _isTriggered = true;
                Debug.Log("Triggered: " + transform.name);
                OnTrigger?.Invoke();
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
