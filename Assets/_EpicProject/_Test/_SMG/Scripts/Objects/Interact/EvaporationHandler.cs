using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

public class EvaporationHandler : MonoBehaviour
{
    bool _isEvaporated = false;

    public UnityEvent OnEvaporate;

    public void Evaporate()
    {
        if (_isEvaporated) return;

        _isEvaporated = true;
        Tilemap tilemap;
        if(TryGetComponent<Tilemap>(out tilemap))
        {
            StartCoroutine(EvaporateCoroutine(tilemap));
        }
        else
        {
            gameObject.SetActive(false);
            OnEvaporate?.Invoke();
        }
        
    }

    IEnumerator EvaporateCoroutine(Tilemap tilemap)
    {
        Color color = tilemap.color;
        
        color.a = 0.8f;
        tilemap.color = color;

        yield return new WaitForSeconds(1f);
        color.a = 0.5f;
        tilemap.color = color;

        yield return new WaitForSeconds(1f);
        color.a = 0.2f;
        tilemap.color = color;

        yield return new WaitForSeconds(1f);
        color.a = 0.0f;
        tilemap.color = color;

        gameObject.SetActive(false);
        OnEvaporate?.Invoke();
    }
}
