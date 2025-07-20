using System.Collections.Generic;
using UnityEngine;

public class PassageSizeGate : MonoBehaviour
{
    public float xThreshold = 0.3f;
    private List<Movement2D> _list = new List<Movement2D>();

    private void Update()
    {
        foreach (Movement2D movement2D in _list)
        {

            if (movement2D.GetComponentInChildren<FootTag>().GetComponent<Collider2D>().bounds.extents.x > xThreshold)
            {
                movement2D.MultiplySpeed(0f);
            }
            else
            {
                movement2D.MultiplySpeed(1f);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<FootTag>(out FootTag footTag))
        {
            Movement2D movement2D = footTag.GetComponentInParent<Movement2D>();
            if (!_list.Contains(movement2D))
            {
                _list.Add(movement2D);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<FootTag>(out FootTag footTag))
        {
            Movement2D movement2D = footTag.GetComponentInParent<Movement2D>();
            if (_list.Contains(movement2D))
            {
                _list.Remove(movement2D);
            }
        }
    }
}
