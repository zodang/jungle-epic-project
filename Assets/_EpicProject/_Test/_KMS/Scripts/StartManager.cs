using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // ← 추가
using System.Collections;

public class StartManager : MonoBehaviour
{
    [Header("Drag & Drop Settings")]
    public LayerMask draggableLayer;
    public LayerMask dropZoneLayer;

    [Header("Fade Settings")]
    public Image fadeImage;
    public float fadeDuration = 1f;
    public float fadeDelay = 1f;      // 페이드 시작 전 딜레이 시간

    public Animator _startAni;

    private Transform _dragging;
    private Vector3 _dragOffset;

    void Start()
    {
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 wp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var hit = Physics2D.Raycast(wp, Vector2.zero, Mathf.Infinity, draggableLayer);
            if (hit.collider != null)
            {
                _dragging = hit.collider.transform;
                _dragOffset = _dragging.position - (Vector3)wp;
            }
        }

        if (_dragging != null)
        {
            if (Input.GetMouseButton(0))
            {
                Vector2 wp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                _dragging.position = wp + (Vector2)_dragOffset;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                Vector2 wp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                var dropHit = Physics2D.Raycast(wp, Vector2.zero, Mathf.Infinity, dropZoneLayer);
                if (dropHit.collider != null)
                {
                    _dragging.position = dropHit.collider.transform.position;
                    // 애니메이션 재생
                    _startAni.Play("Start Ani");
                    // 1초 딜레이 후 페이드 시작
                    StartCoroutine(DelayedFade(fadeDelay));
                }
                _dragging = null;
            }
        }
    }

    private IEnumerator DelayedFade(float delay)
    {
        yield return new WaitForSeconds(delay);
        yield return StartCoroutine(FadeToBlack());
    }

    private IEnumerator FadeToBlack()
    {
        Color c = fadeImage.color;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Clamp01(elapsed / fadeDuration);
            fadeImage.color = c;
            yield return null;
        }

        // 페이드 완료 후 1초 대기
        yield return new WaitForSeconds(1f);
        // MenuScene 로드 (씬 이름을 프로젝트에 맞게 바꿔주세요)
        SceneManager.LoadScene("MenuScene");
    }
}
