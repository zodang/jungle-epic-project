using UnityEngine;

public class GlitchObject : MonoBehaviour
{
    [Header("Materials")]
    public Material defaultMat;
    public Material glitchMat;
    
    [Header("Boolean isGlitchVision")]
    public bool isGlitchVision = false; // GlitchVision 모드 여부

    [Header("Glitch Effect Settings")]
    public float glitchChangeSpeed = 0.1f; // 1초에 1.0씩 변화



    private float currentGlitchValue = 0.1f;
    private float targetGlitchValue = 0.1f;
    private SpriteRenderer spriteRenderer;



    private void Start()
    {
        // 시작시 spriteRenderer에 defaultMat를 적용
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && defaultMat != null)
            spriteRenderer.material = defaultMat;

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

        // targetGlitchValue을 새 랜덤값으로 지정
        targetGlitchValue = Random.Range(-1f, -0.5f);
    }

    public void UpdateGlitchEffectValue()
    {
        // 목표값 도달시, 새로운 목표값 지정
        if (Mathf.Approximately(currentGlitchValue, targetGlitchValue))
        {
            targetGlitchValue = Random.Range(-1f, -0.5f);
        }

        // 서서히 목표값을 향해 이동
        currentGlitchValue = Mathf.MoveTowards(currentGlitchValue, targetGlitchValue, glitchChangeSpeed * Time.deltaTime);

        // 값 적용
        if (glitchMat != null)
            glitchMat.SetFloat("_GlitchEffect", currentGlitchValue);
    }
}
