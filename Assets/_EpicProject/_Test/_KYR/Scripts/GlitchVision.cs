using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;
using System.Collections;

public class GlitchVision : MonoBehaviour
{
    [Header("Glitch Vision Settings")]
    public float glitchVisionDuration = 3f; // GlitchVision 지속 시간
    public float startFastSpeed = 0.05f; // GlitchVision 시작 속도(값 클수록 빠름)
    public float endSlowSpeed = 2f; // GlitchVision 종료 속도(값 클수록 느림)

    [Header("Glitch Volume")]
    public Volume glitchVolume;

    public List<GlitchObject> glitchObjects;
    private Coroutine glitchCoroutine;


    private void Start()
    {
        // GlitchObject 리스트 초기화 및 추가
        glitchObjects = new List<GlitchObject>(FindObjectsByType<GlitchObject>(FindObjectsSortMode.None));

        // glitchVolume 초기화
        if (glitchVolume != null)
        {
            glitchVolume.gameObject.SetActive(false);
            glitchVolume.weight = 0f;
        }
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            // Q 키를 눌렀을 때 GlitchVision 활성화
            ActivateGlitchVision();

            // glitchVolume 코루틴이 이미 실행 중이면 중지하고 새로 시작
            if (glitchCoroutine != null)
                StopCoroutine(glitchCoroutine);
            glitchCoroutine = StartCoroutine(GlitchVolumeRoutine());
        }
    }


    public void ActivateGlitchVision()
    {
        foreach (var obj in glitchObjects)
        {
            // 모든 GlitchObject에 대해 ShowGlitch 호출
            if (obj != null)
                obj.ShowGlitch();
        }
    }

    public void DeactivateGlitchVision()
    {
        foreach (var obj in glitchObjects)
        {
            // 모든 GlitchObject에 대해 HideGlitch 호출
            if (obj != null)
                obj.HideGlitch();
        }
    }

    private IEnumerator GlitchVolumeRoutine()
    {
        // glitchVolume이 null인 경우 종료
        if (glitchVolume == null)
            yield break;

        // glitchVolume Active
        glitchVolume.gameObject.SetActive(true);

        float halfDuration = glitchVisionDuration * 0.5f;
        float t = 0f;

        // 빠르게 증가 (EaseOut)
        while (t < halfDuration)
        {
            float progress = t / halfDuration;
            float eased = Mathf.Pow(progress, startFastSpeed);
            glitchVolume.weight = Mathf.Lerp(0f, 1f, eased);
            t += Time.deltaTime;
            yield return null;
        }
        glitchVolume.weight = 1f;

        // 느리게 감소 (EaseIn)
        t = 0f;
        while (t < halfDuration)
        {
            float progress = t / halfDuration;
            float eased = Mathf.Pow(progress, endSlowSpeed);
            glitchVolume.weight = Mathf.Lerp(1f, 0f, eased);
            t += Time.deltaTime;
            yield return null;
        }
        glitchVolume.weight = 0f;

        // glitchVolume Deactive
        glitchVolume.gameObject.SetActive(false);

        // GlitchObject 효과 종료
        DeactivateGlitchVision();
    }

}
