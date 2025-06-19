// TypeEffect.cs (리치 텍스트 지원하도록 수정됨)
using UnityEngine;
using TMPro;
using System.Collections;
using System.Text; // StringBuilder를 사용하기 위해 추가
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

        // 리치 텍스트를 사용하려면 반드시 true여야 합니다.
        if (msgText != null)
        {
            msgText.richText = true;
        }
    }

    private void Start()
    {
        // GameManager 인스턴스가 생성된 이후에 AudioManager를 참조하도록 변경할 수 있습니다.
        // 만약 Start 시점에 GameManager.Instance가 아직 null일 수 있다면, EffectStart에서 참조를 가져오는 것이 더 안정적일 수 있습니다.
        audioManager = GameManager.Instance.AudioManager;
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
        IsPlaying = true;
        if (charPerSeconds <= 0 || string.IsNullOrEmpty(targetMsg))
        {
            msgText.text = targetMsg;
            EffectEnd();
            return;
        }
        typingCoroutine = StartCoroutine(EffectRoutine());
    }

    // ==================================================================
    // 여기가 수정된 핵심 로직입니다.
    // ==================================================================
    IEnumerator EffectRoutine()
    {
        StringBuilder stringBuilder = new StringBuilder();
        int currentIndex = 0;

        while (currentIndex < targetMsg.Length)
        {
            // 리치 텍스트 태그 '<'를 만났는지 확인
            if (targetMsg[currentIndex] == '<')
            {
                int endIndex = targetMsg.IndexOf('>', currentIndex);
                if (endIndex != -1)
                {
                    // 태그 전체(<...>)를 한 번에 추가 (소리, 딜레이 없음)
                    string tag = targetMsg.Substring(currentIndex, endIndex - currentIndex + 1);
                    stringBuilder.Append(tag);

                    // 인덱스를 태그 끝 다음으로 점프
                    currentIndex = endIndex + 1;

                    // 화면 텍스트 업데이트 후 다음 루프로 바로 넘어감
                    msgText.text = stringBuilder.ToString();
                    continue;
                }
            }

            // 일반 문자인 경우, 한 글자씩 추가
            stringBuilder.Append(targetMsg[currentIndex]);
            msgText.text = stringBuilder.ToString();

            // ========== 효과음 재생 (태그가 아닐 때만 재생됨) ==========
            if (audioManager != null && targetMsg[currentIndex] != ' ')
            {
                audioManager.PlaySfx(SfxType.Text);
            }
            // =====================================================

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
        EffectEnd();
    }

    void EffectEnd()
    {
        IsPlaying = false;
        typingCoroutine = null;
    }

    public void FinishEffect()
    {
        if (!enabled || msgText == null) return;
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);

        // FinishEffect가 호출될 때도 리치 텍스트가 적용된 최종본이 보여야 하므로 targetMsg를 그대로 사용
        msgText.text = targetMsg;
        EffectEnd();
    }
}