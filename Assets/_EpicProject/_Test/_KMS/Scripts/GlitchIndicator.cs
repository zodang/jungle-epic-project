using UnityEngine;
using System.Collections;

public class GlitchIndicator : MonoBehaviour
{
    // 글리치 종료 신호를 받기 위한 플래그
    private bool _glitchEnded = false;

    private void Start()
    {
        GlitchVision.BeginGlitch += StartGlitchTimer;
        GlitchVision.EndGlitch += OnGlitchEnd;
    }

    private void OnDestroy()
    {
        GlitchVision.BeginGlitch -= StartGlitchTimer;
        GlitchVision.EndGlitch -= OnGlitchEnd;
    }

    // BeginGlitch 이벤트에 연결
    private void StartGlitchTimer()
    {
        Debug.Log("글리치비전 시작됨");
        _glitchEnded = false;
        StartCoroutine(GlitchTimerCoroutine());
    }

    // EndGlitch 이벤트에 연결
    private void OnGlitchEnd()
    {
        _glitchEnded = true;
    }

    private IEnumerator GlitchTimerCoroutine()
    {
        // 1초마다 로그 출력
        for (int i = 1; i <= 3; i++)
        {
            yield return new WaitForSeconds(1f);
            Debug.Log($"{i}초..");
        }

        // 여기서 EndGlitch 이벤트가 올 때까지 대기
        yield return new WaitUntil(() => _glitchEnded);

        Debug.Log("글리치비전 끝남");
    }
}
