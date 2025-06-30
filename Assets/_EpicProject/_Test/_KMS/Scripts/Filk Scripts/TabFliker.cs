using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Image))]
public class TabFliker : MonoBehaviour
{
    [SerializeField] private Image _tapImage;
    private Coroutine _flickerCoroutine;

    // 깜빡임 주기
    private const float FlickerInterval = 0.3f;
    // 최대 불투명도
    private const float FlickerAlpha = 0.6f;

    private void Awake()
    {
        if (_tapImage == null)
            _tapImage = GetComponent<Image>();

        // 페이드 기능을 쓰려면 캔버스 렌더러 알파부터 초기화
        _tapImage.canvasRenderer.SetAlpha(1f);
    }

    private void Start()
    {
        DraggableUI.OnDragBeginEngine += BeginFlik;
        DraggableUI.OnDragEndEngine += EndFlik;
    }

    private void OnDestroy()
    {
        DraggableUI.OnDragBeginEngine -= BeginFlik;
        DraggableUI.OnDragEndEngine -= EndFlik;
    }

    private void BeginFlik()
    {
        if (_flickerCoroutine == null)
            _flickerCoroutine = StartCoroutine(FlickerCoroutine());
    }

    private void EndFlik()
    {
        if (_flickerCoroutine != null)
        {
            StopCoroutine(_flickerCoroutine);
            _flickerCoroutine = null;
            // 깜빡임 끝나면 완전 불투명으로 복귀
            _tapImage.CrossFadeAlpha(1f, 0f, false);
        }
    }

    private IEnumerator FlickerCoroutine()
    {
        while (true)
        {
            // 0 → 0.6으로 부드럽게
            _tapImage.CrossFadeAlpha(FlickerAlpha, FlickerInterval, false);
            yield return new WaitForSeconds(FlickerInterval);
            // 0.6 → 0으로 부드럽게
            _tapImage.CrossFadeAlpha(0f, FlickerInterval, false);
            yield return new WaitForSeconds(FlickerInterval);
        }
    }
}
