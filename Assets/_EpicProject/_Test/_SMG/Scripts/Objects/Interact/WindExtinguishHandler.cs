using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class WindExtinguishHandler : MonoBehaviour
{
    Dictionary<GameObject, IWindEmitter> emitterCache = new();
    List<Collider2D> list = new List<Collider2D>();
    
    public UnityEvent<float> OnApplyWind;

    private void Update()
    {
        foreach(var pair in emitterCache)
        {
            if (pair.Key.IsUnityNull()) continue;
            OnApplyWind?.Invoke(pair.Value.GetWindPower() * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("WindTrigger"))
        {
            emitterCache.TryAdd(collision.gameObject, collision.transform.root.GetComponentInChildren<IWindEmitter>());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("WindTrigger"))
        {
            emitterCache.Remove(collision.gameObject);
        }
    }
}
