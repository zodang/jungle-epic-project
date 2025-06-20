using UnityEngine;
using UnityEngine.Rendering;
using Unity.Cinemachine;
using System.Collections.Generic;
using System.Collections;

public class GlitchVision : MonoBehaviour
{
    [Header("Glitch Vision Settings")]
    public float glitchVisionDuration = 3f; // GlitchVision 지속 시간
    public float startFastSpeed = 0.05f; // GlitchVision 시작 속도(값 클수록 빠름)
    public float endSlowSpeed = 2f; // GlitchVision 종료 속도(값 클수록 느림)

    // [Header("Camera Settings")]
    // public CinemachineCamera mainCam;
    // public float glitchCamOrtho = 80f; // GlitchVision OrthographicSize
    // private float mainCamOrtho;

    [Header("Glitch Volume")]
    public Volume glitchVolume;

    [Header("Audio Filter")]
    private AudioLowPassFilter lowPassFilter;
    public float normalCutoff = 5000f; // 원래 값 (기본값)
    public float glitchCutoff = 400f;   // 글리치 시 먹먹한 값

    public List<GlitchObject> glitchObjects;
    private Coroutine glitchCoroutine;
    private Coroutine lowPassRoutine;


    private void Start()
    {
        // mainCam의 fov 저장
        // mainCamOrtho = mainCam.Lens.OrthographicSize;
        // Debug.Log("Main Camera FOV: " + mainCamOrtho);

        // GlitchObject 리스트 초기화 및 추가
        glitchObjects = new List<GlitchObject>(FindObjectsByType<GlitchObject>(FindObjectsSortMode.None));

        // glitchVolume 초기화
        if (glitchVolume != null)
        {
            glitchVolume.gameObject.SetActive(false);
            glitchVolume.weight = 0f;
        }

        // AudioLowPassFilter 찾기 (AudioLowPassFilter is must attached on CinemachineBrain)
        lowPassFilter = FindFirstObjectByType<AudioLowPassFilter>();
        if (lowPassFilter != null)
            normalCutoff = lowPassFilter.cutoffFrequency;
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

            // LowPassRoutine 코루틴이 이미 실행 중이면 중지하고 새로 시작
            if (lowPassFilter != null)
                StartCoroutine(LowPassRoutine());
            lowPassRoutine = StartCoroutine(GlitchVolumeRoutine());
        }
    }


    public void ActivateGlitchVision()
    {
        // 카메라 OrthographicSize 확장
        // mainCam.Lens.FieldOfView = glitchCamOrtho;

        foreach (var obj in glitchObjects)
        {
            // 모든 GlitchObject에 대해 ShowGlitch 호출
            if (obj != null)
                obj.ShowGlitch();
        }
    }

    public void DeactivateGlitchVision()
    {
        // 카메라 OrthographicSize 복귀
        // mainCam.Lens.FieldOfView = mainCamOrtho;

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

    private IEnumerator LowPassRoutine()
    {
        // lowPassFilter가 null인 경우 종료
        if (lowPassFilter == null)
            yield break;

        float halfDuration = glitchVisionDuration * 0.5f;
        float t = 0f;

        // 빠르게 cutoff 감소 (먹먹하게, EaseOut)
        while (t < halfDuration)
        {
            float progress = t / halfDuration;
            float eased = Mathf.Pow(progress, startFastSpeed);
            lowPassFilter.cutoffFrequency = Mathf.Lerp(normalCutoff, glitchCutoff, eased);
            t += Time.deltaTime;
            yield return null;
        }
        lowPassFilter.cutoffFrequency = glitchCutoff;

        // 느리게 cutoff 복귀 (EaseIn)
        t = 0f;
        while (t < halfDuration)
        {
            float progress = t / halfDuration;
            float eased = Mathf.Pow(progress, endSlowSpeed);
            lowPassFilter.cutoffFrequency = Mathf.Lerp(glitchCutoff, normalCutoff, eased);
            t += Time.deltaTime;
            yield return null;
        }
        lowPassFilter.cutoffFrequency = normalCutoff;
    }   
}
