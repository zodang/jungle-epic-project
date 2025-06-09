using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EvaporationHandler : MonoBehaviour
{
    public void Evaporate()
    {
        Tilemap tilemap;
        if(TryGetComponent<Tilemap>(out tilemap))
        {
            StartCoroutine(EvaporateCoroutine(tilemap));
        }
        else
        {
            gameObject.SetActive(false);
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
    }
}
