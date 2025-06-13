// TypeEffect.cs (코루틴 및 WaitForSecondsRealtime 사용)
using UnityEngine;
using TMPro;
using System.Collections;
using Define;

public class TypeEffect : MonoBehaviour
{
    public string targetMsg;
    public float charPerSeconds = 15f;
    TextMeshProUGUI msgText;
    private AudioManager audioManager; // AudioManager 참조
    public bool IsPlaying { get; private set; }

    private Coroutine typingCoroutine;

    private void Awake()
    {
        msgText = GetComponent<TextMeshProUGUI>();
        if (msgText == null) { enabled = false; Debug.LogError($"TypeEffect CRITICAL ERROR: TextMeshProUGUI not found on '{gameObject.name}'.", gameObject); }
        audioManager = AudioManager.Instance;
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
        int currentIndex = 0;
        while (currentIndex < targetMsg.Length)
        {
            msgText.text += targetMsg[currentIndex];

            // ========== 효과음 재생 ==========
            if (audioManager != null && targetMsg[currentIndex] != ' ')
            {
                audioManager.PlaySfx(SfxType.Text); // <--- SfxType.DialogueType 정의 필요
            }
            // ==============================

            currentIndex++;
            if (charPerSeconds > 0)
            {
                yield return new WaitForSecondsRealtime(1.0f / charPerSeconds);
            }
            else
            {
                yield return null;
            }
        }
        EffectEnd(); // 코루틴에서는 EffectEnd 대신 CompleteEffect (IsPlaying 등 처리)
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