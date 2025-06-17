using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BridgePath : MonoBehaviour
{
    List<Collider2D> _overlappingColliders = new List<Collider2D>();
    List<Collider2D> _enteredColliders = new List<Collider2D>();

    void AddColliderToList(List<Collider2D> list, Collider2D collider)
    {
        if (!list.Contains(collider))
        {
            list.Add(collider);
        }
    }

    void RemoveColliderToList(List<Collider2D> list, Collider2D collider)
    {
        if (list.Contains(collider))
        {
            list.Remove(collider);
        }
    }

    void SetIgnoreCollision(List<Collider2D> list, Collider2D collider, bool ignore = true)
    {
        for (int i = 0; i < list.Count; i++)
        {
            Physics2D.IgnoreCollision(list[i], collider, ignore);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Movement2D movement = collision.GetComponentInParent<Movement2D>();
        if (!movement.IsUnityNull())// collision.CompareTag("Player"))
        {
            movement.CheckGround = true;
            //Debug.Log("Player Enter Bridge");
            if (!_enteredColliders.Contains(collision))
            {
                _enteredColliders.Add(collision);
            }

            SetIgnoreCollision(_overlappingColliders, collision);
        }
        else
        {
            //Debug.Log("OnTriggerEnter2D: " + collision.name);
            if (!_overlappingColliders.Contains(collision))
            {
                _overlappingColliders.Add(collision);
                SetIgnoreCollision(_enteredColliders, collision);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Movement2D movement = collision.GetComponentInParent<Movement2D>();
        if (!movement.IsUnityNull()) //if (collision.CompareTag("Player"))
        {
            movement.CheckGround = false;
            //Debug.Log("Player Exit Bridge");
            if (_enteredColliders.Contains(collision))
            {
                _enteredColliders.Remove(collision);
            }

            SetIgnoreCollision(_overlappingColliders, collision, false);
        }
        else
        {
            //Debug.Log("OnTriggerExit2D: " + collision.name);
            if (_overlappingColliders.Contains(collision))
            {
                _overlappingColliders.Remove(collision);
                SetIgnoreCollision(_enteredColliders, collision, false);
            }
        }
    }
}
