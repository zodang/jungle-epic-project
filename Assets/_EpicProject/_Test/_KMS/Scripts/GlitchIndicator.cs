using UnityEngine;
using System.Collections;
using DG.Tweening;
using Unity.VisualScripting;
using TMPro;
using UnityEngine.UI;

public class GlitchIndicator : MonoBehaviour
{
    [Header("Animator")]
    [SerializeField] private Animator _glitchAni;

    [Header("Cooldown Image (RectTransform)")]
    [SerializeField] private RectTransform _cooldownImageRT;

    [Header("DOTween 세팅")]
    [SerializeField] private float _tweenDuration = 0.3f;
    private float _raiseY = 300f;  // 앵커 Y 이동량

    public Vector2 _originalAnchoredPos;
    private bool _glitchEnded = false;

    private DG.Tweening.Sequence sequence;


    private void Start()
    {
        _originalAnchoredPos = _cooldownImageRT.anchoredPosition; 
        GlitchVision.BeginGlitch += StartGlitchTimer;
        GlitchVision.EndGlitch += OnGlitchEnd;
    }

    private void OnDestroy()
    {
        GlitchVision.BeginGlitch -= StartGlitchTimer;
        GlitchVision.EndGlitch -= OnGlitchEnd;
    }

    private void StartGlitchTimer()
    {
        _glitchEnded = false;

        // UI용 DOTween: 앵커 포지션 Y 값 변경
        sequence = DOTween.Sequence();

        sequence
            .OnStart(() => { _glitchAni.Play("Glitch Count"); })
            .Append(_cooldownImageRT.DOAnchorPosY(_originalAnchoredPos.y + _raiseY, _tweenDuration).SetEase(Ease.OutCubic))
            .AppendInterval(3)
            .Append(_cooldownImageRT.DOAnchorPosY(_originalAnchoredPos.y, _tweenDuration).SetEase(Ease.InCubic))
            .OnComplete(() => { _glitchAni.Play("Empty");  _glitchEnded = true; })
       ;

    }

    private void OnGlitchEnd()
    {
        _glitchEnded = true;
    }

    //private IEnumerator GlitchTimerCoroutine()
    //{
    //    _glitchAni.Play("Glitch Count");
    //    yield return new WaitUntil(() => _glitchEnded);

    //    // 다시 원위치로 내리기
    //    _cooldownImageRT
    //        .DOAnchorPosY(_originalAnchoredPos.y, _tweenDuration)
    //        .SetEase(Ease.InCubic);

    //}
}
