// TypeEffect.cs (코루틴 및 WaitForSecondsRealtime 사용)
using UnityEngine;
using TMPro;
using System.Collections;

public class TypeEffect : MonoBehaviour
{
    public string targetMsg;
    public float charPerSeconds = 15f;
    TextMeshProUGUI msgText;
    public bool IsPlaying { get; private set; }

    private Coroutine typingCoroutine;

    private void Awake()
    {
        msgText = GetComponent<TextMeshProUGUI>();
        if (msgText == null) { enabled = false; Debug.LogError($"TypeEffect CRITICAL ERROR: TextMeshProUGUI not found on '{gameObject.name}'.", gameObject); }
    }

    public void SetMsg(string msg)
    {
        if (!enabled || msgText == null) return;
        targetMsg = msg ?? "";
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        EffectStart();
    }

    void EffectStart()
    {
        msgText.text = "";
        IsPlaying = true; // 효과 시작 전 true로 설정
        if (charPerSeconds <= 0 || string.IsNullOrEmpty(targetMsg))
        {
            msgText.text = targetMsg;
            EffectEnd(); // IsPlaying = false; 호출됨
            return;
        }
        typingCoroutine = StartCoroutine(EffectRoutine());
    }

    IEnumerator EffectRoutine()
    {
        // Debug.Log("<TypeEffect> Coroutine Started. Target: " + targetMsg);
        // IsPlaying = true; // EffectStart에서 이미 설정됨
        int currentIndex = 0;
        while (currentIndex < targetMsg.Length)
        {
            msgText.text += targetMsg[currentIndex];
            currentIndex++;
            if (charPerSeconds > 0) // 0으로 나누기 방지
            {
                yield return new WaitForSecondsRealtime(1.0f / charPerSeconds);
            }
            else // 속도가 0 이하면 한 프레임에 한 글자씩 (매우 빠름) 또는 즉시 완료 로직 필요
            {
                yield return null; // 다음 프레임까지 대기 (또는 즉시 완료하도록 로직 수정)
            }
        }
        EffectEnd();
    }

    void EffectEnd()
    {
        IsPlaying = false;
        typingCoroutine = null;
        // Debug.Log("<TypeEffect> Coroutine Ended or Effect Finished. IsPlaying: " + IsPlaying);
    }

    public void FinishEffect()
    {
        if (!enabled || msgText == null) return;
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        msgText.text = targetMsg;
        EffectEnd(); // IsPlaying = false 및 typingCoroutine = null 처리
    }
}