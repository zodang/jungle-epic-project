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

    [Header("Camera Settings")]
    public CinemachineCamera mainCam;
    private CinemachineCamera _glitchCam;
    public bool isZoomOut = true;
    private float mainCamOrtho; // mainCam의 OrthographicSize 저장
    public float glitchCamOrtho = 6.5f; // GlitchCam의 OrthographicSize

    [Header("Glitch Volume")]
    private Volume _glitchVolume;

    [Header("Audio Filter")]
    public float normalCutoff = 5000f; // 원래 값 (기본값)
    public float glitchCutoff = 400f;   // 글리치 시 먹먹한 값
    private AudioLowPassFilter lowPassFilter;

    private List<GlitchObject> _glitchObjects;
    private Coroutine glitchCoroutine;
    private Coroutine lowPassRoutine;

    private void Awake()
    {
        _glitchCam = GetComponent<CinemachineCamera>();
        _glitchVolume = transform.GetComponentInChildren<Volume>();
        _glitchObjects = new List<GlitchObject>(FindObjectsByType<GlitchObject>(FindObjectsSortMode.None));
    }

    private void Start()
    {
        // mainCam의 ortho 저장, glitchCam, glitchCam Group Framing의 ortho 설정
        mainCamOrtho = mainCam.Lens.OrthographicSize;
        _glitchCam.Lens.OrthographicSize = glitchCamOrtho;
        _glitchCam.GetComponent<CinemachineGroupFraming>().OrthoSizeRange = new Vector2(glitchCamOrtho, 10);
        Debug.Log("Main Camera OrthographicSize: " + mainCamOrtho);
        Debug.Log("Glitch Camera OrthographicSize: " + glitchCamOrtho);

        // GlitchObject 리스트 초기화 및 추가
        _glitchObjects = new List<GlitchObject>(FindObjectsByType<GlitchObject>(FindObjectsSortMode.None));

        // glitchVolume 초기화
        if (_glitchVolume != null)
        {
            _glitchVolume.gameObject.SetActive(false);
            _glitchVolume.weight = 0f;
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
                StopCoroutine(LowPassRoutine());
            lowPassRoutine = StartCoroutine(LowPassRoutine());
        }
    }


    public void ActivateGlitchVision()
    {
        if (isZoomOut)
        {
            // 카메라 전환
            mainCam.Priority = 0;
            _glitchCam.Priority = 10;
        }

        foreach (var obj in _glitchObjects)
        {
            // 모든 GlitchObject에 대해 ShowGlitch 호출
            if (obj != null)
                obj.ShowGlitch();
        }
    }

    public void DeactivateGlitchVision()
    {
        if (isZoomOut)
        {
            // 카메라 복귀
            mainCam.Priority = 10;
            _glitchCam.Priority = 0;
        }

        foreach (var obj in _glitchObjects)
        {
            // 모든 GlitchObject에 대해 HideGlitch 호출
            if (obj != null)
                obj.HideGlitch();
        }
    }

    private IEnumerator GlitchVolumeRoutine()
    {
        // glitchVolume이 null인 경우 종료
        if (_glitchVolume == null)
            yield break;

        // glitchVolume Active
        _glitchVolume.gameObject.SetActive(true);

        float halfDuration = glitchVisionDuration * 0.5f;
        float t = 0f;

        // 빠르게 증가 (EaseOut)
        while (t < halfDuration)
        {
            float progress = t / halfDuration;
            float eased = Mathf.Pow(progress, startFastSpeed);
            _glitchVolume.weight = Mathf.Lerp(0f, 1f, eased);
            t += Time.deltaTime;
            yield return null;
        }
        _glitchVolume.weight = 1f;

        // 느리게 감소 (EaseIn)
        t = 0f;
        while (t < halfDuration)
        {
            float progress = t / halfDuration;
            float eased = Mathf.Pow(progress, endSlowSpeed);
            _glitchVolume.weight = Mathf.Lerp(1f, 0f, eased);
            t += Time.deltaTime;
            yield return null;
        }
        _glitchVolume.weight = 0f;

        // glitchVolume Deactive
        _glitchVolume.gameObject.SetActive(false);

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
