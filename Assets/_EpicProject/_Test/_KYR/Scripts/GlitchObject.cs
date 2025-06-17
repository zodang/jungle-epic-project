using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public class GlitchObject : MonoBehaviour
{
    [Header("Materials")]
    public Material defaultMat;
    public Material glitchMat;
    
    [Header("Boolean isGlitchVision")]
    public bool isGlitchVision = false; // GlitchVision 모드 여부

    [Header("Glitch Effect Settings")]
    public float glitchValueUpdateSpeed = 1f;
    public float glitchChangeInterval = 0.2f;

    private float currentGlitchValue = 0.1f;
    private float targetGlitchValue = 0.1f;
    private SpriteRenderer spriteRenderer;
    private Coroutine glitchApplyCoroutine;


    private void Start()
    {
        // 시작시 spriteRenderer에 defaultMat를 적용
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && defaultMat != null)
            spriteRenderer.material = defaultMat;

        // glitchMat 인스턴스화
        if (glitchMat != null)
            glitchMat = new Material(glitchMat);

        // Shader _GlitchEffect 변수 초기값 적용
        if (glitchMat != null)
            glitchMat.SetFloat("_GlitchEffect", currentGlitchValue);
    }

    private void Update()
    {
        if (isGlitchVision)
        {
            UpdateGlitchEffectValue();
        }
    }

    public void ShowGlitch()
    {
        // glitchMat으로 변경
        spriteRenderer.material = glitchMat;

        // GlitchVision 모드 활성화
        isGlitchVision = true;

        // GlitchVision Value 초기화
        glitchMat.SetFloat("_ChromAberrAmountX", Random.Range(-0.3f, 0.3f));
        glitchMat.SetFloat("_ChromAberrAmountY", 0);
        glitchMat.SetVector("_DisplacementAmount", new Vector4(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), 0));
        glitchMat.SetFloat("_RightStripesAmount", Random.Range(10f, 30f));
        glitchMat.SetFloat("_RightStripesFill", Random.Range(0.5f, 0.7f));
        glitchMat.SetFloat("_LeftStripesAmount", Random.Range(10f, 30f));
        glitchMat.SetFloat("_LeftStripesFill", Random.Range(0.5f, 0.7f));
        glitchMat.SetFloat("_WavyDisplFreq", Random.Range(-1f, 1f));

        // targetGlitchValue을 새 랜덤값으로 지정
        targetGlitchValue = Random.Range(-1f, -0.5f);

        // 이전 코루틴을 중지하고, ApplyGlitchEffectRoutine 시작
        if (glitchApplyCoroutine != null)
            StopCoroutine(glitchApplyCoroutine);
        glitchApplyCoroutine = StartCoroutine(ApplyGlitchEffectRoutine());
    }

    public void HideGlitch()
    {
        // defaultMat으로 변경
        spriteRenderer.material = defaultMat;

        // GlitchVision 모드 비활성화
        isGlitchVision = false;

        // 코루틴 정지
        if (glitchApplyCoroutine != null)
        {
            StopCoroutine(glitchApplyCoroutine);
            glitchApplyCoroutine = null;
        }
    }

    public void UpdateGlitchEffectValue()
    {
        // 목표값 도달시, 새로운 목표값 지정
        if (Mathf.Approximately(currentGlitchValue, targetGlitchValue))
        {
            targetGlitchValue = Random.Range(-1f, -0.5f);
        }

        // 서서히 목표값을 향해 이동
        currentGlitchValue = Mathf.MoveTowards(currentGlitchValue, targetGlitchValue, glitchValueUpdateSpeed * Time.deltaTime);
    }

    private IEnumerator ApplyGlitchEffectRoutine()
    {
        while (isGlitchVision && glitchMat != null)
        {
            glitchMat.SetFloat("_GlitchEffect", currentGlitchValue);
            yield return new WaitForSeconds(glitchChangeInterval);
        }
    }
}
