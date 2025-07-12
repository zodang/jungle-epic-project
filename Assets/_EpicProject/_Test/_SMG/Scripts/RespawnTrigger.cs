using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(TriggerArea))]
public class RespawnTrigger : MonoBehaviour
{
    public Transform Point;
    private TriggerArea _triggerArea;

    private void Awake()
    {
        _triggerArea = GetComponent<TriggerArea>();
    }

    public void Movement2DRespawn()
    {
        if (Point.IsUnityNull()) return;

        Movement2D movement2D = _triggerArea.target.GetComponent<Movement2D>();
        
        movement2D.Respawn(Point.position);
    }

}
