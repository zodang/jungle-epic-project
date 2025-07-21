using System.Collections;
using Unity.Services.Analytics;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public struct DelayUnityEvent
{
    public UnityEvent myEvent;
    public float Delay;
}

public class DeltaDelayInvoker : MonoBehaviour
{
    public DelayUnityEvent[] DelayUnityEvents;

    public void StartSequence()
    {
        for(int i = 0; i < DelayUnityEvents.Length; i++)
        {
            StartCoroutine(DelayEventCoroutine(DelayUnityEvents[i].myEvent, DelayUnityEvents[i].Delay));
        }
    }

    IEnumerator DelayEventCoroutine(UnityEvent unityEvent, float delay)
    {
        yield return new WaitForSeconds(delay);
        unityEvent?.Invoke();
    }

}
