using System.Collections.Generic;
using UnityEngine;

public class PassageSizeBlocker : MonoBehaviour
{
    public AxisLock BlockDir;

    public float xThreshold = 0.139f;
    private List<Movement2D> _list = new List<Movement2D>();

    private void Update()
    {
        foreach (Movement2D movement2D in _list)
        {
            if (movement2D.GetComponentInChildren<FootTag>().GetComponent<Collider2D>().bounds.extents.x > xThreshold)
            {
                movement2D.MoveLockY(BlockDir);
            }
            else
            {
                movement2D.MoveLockY(AxisLock.None);
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
                movement2D.MoveLockY(AxisLock.None);
                _list.Remove(movement2D);
            }
        }
    }
}
