using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // ← 추가
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using Define;

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

    [SerializeField] private Canvas privacyConfirmPopup;
    
    private LocalizeSpriteEvent _keyLocalization;
    private SpriteRenderer[] _spriteRenderers;
    private ArrowWaveSmooth _arrowWaveSmooth;

    private bool _isStarted;

    private void Awake()
    {
        _keyLocalization = FindAnyObjectByType<LocalizeSpriteEvent>();
        _arrowWaveSmooth = FindAnyObjectByType<ArrowWaveSmooth>();
        _spriteRenderers = FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None);

        // Localization 적용 전 UI 투명화
        _arrowWaveSmooth.ChangeAlpha(0, 0);
        foreach (var spriteRenderer in _spriteRenderers)
        {
            spriteRenderer.color = Color.clear;
        }

        privacyConfirmPopup.enabled = false;
    }

    private void Start()
    {
        StartCoroutine(LocalizationCo());
    }
    
    private IEnumerator LocalizationCo()
    {
        yield return LocalizationSettings.InitializationOperation;
        yield return new WaitForSeconds(0.5f);
        
        // Localization 적용 후 UI 정상화
        _arrowWaveSmooth.ChangeAlpha(0.5f, 1.0f);
        foreach (var spriteRenderer in _spriteRenderers)
        {
            spriteRenderer.color = Color.white;
        }
        
        ShowPrivacyPolicyPopup();
    }

    void Update()
    {
        if (_isStarted) return; // Start Key 재드래그 제한
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return; // 팝업 활성화 시 드래그 제한

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
                    _isStarted = true;
                    
                    _dragging.position = dropHit.collider.transform.position;
                    // 애니메이션 재생
                    _startAni.Play("Start Ani");
                    GameManager.Instance.AudioManager.PlaySfx(SfxType.Open);
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

    // 개인정보 처리방침 안내 팝업
    private void ShowPrivacyPolicyPopup()
    {
        if (GameManager.Instance.SaveManager.LoadPrivacyConfirmData())
        {
            GameManager.Instance.UIManager.ActivateGameUIManager(true);
            return;
        }
        
        privacyConfirmPopup.enabled = true;
        
        string title = LocalizationSettings.StringDatabase.GetLocalizedString("PopupUI", "PrivacyConfirm_Title");
        string message =  LocalizationSettings.StringDatabase.GetLocalizedString("PopupUI", "PrivacyConfirm_Message");
        string okLabel =  LocalizationSettings.StringDatabase.GetLocalizedString("PopupUI", "PrivacyConfirm_Agree");
        string cancelLabel =  LocalizationSettings.StringDatabase.GetLocalizedString("PopupUI", "PrivacyConfirm_Disagree");

        GameManager.Instance.UIManager.PopupUI.ShowPopup
        (
            title,
            message,
            onOk: () =>
            {
                GameManager.Instance.LogManager.OptIn();
                
                GameManager.Instance.SaveManager.SavePrivacyAgreement(true);
                GameManager.Instance.SaveManager.SavePrivacyData(true);
                
                privacyConfirmPopup.enabled = false;
                GameManager.Instance.UIManager.ActivateGameUIManager(true);
            },
            onCancel: () =>
            {
                GameManager.Instance.LogManager.OptOut();
                
                GameManager.Instance.SaveManager.SavePrivacyAgreement(false);
                GameManager.Instance.SaveManager.SavePrivacyData(true);
                
                privacyConfirmPopup.enabled = false;
                GameManager.Instance.UIManager.ActivateGameUIManager(true);
            },
            okLabel,
            cancelLabel
        );
    }
}
