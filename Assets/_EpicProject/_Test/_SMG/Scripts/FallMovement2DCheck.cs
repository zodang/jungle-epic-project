using Unity.VisualScripting;
using UnityEngine;

public class FallMovement2DCheck : MonoBehaviour
{
    public Transform ReaspwanPoint;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Movement2D movement2D;
        movement2D = collision.GetComponentInParent<Movement2D>();
        if (!movement2D.IsUnityNull())
        {
            movement2D.Respawn(ReaspwanPoint.position);
        }
        //if(collision.transform.root.TryGetComponent<Movement2D>(out Movement2D movement))
        //{
        //    Debug.Log(name + ".OnTriggerEnter2D: " + collision.name);
        //    movement.Reaspawn(ReaspwanPoint.position);
        //}
        
    }
}
