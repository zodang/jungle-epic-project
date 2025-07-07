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

    public Vector2 _originalAnchoredPos;
    private bool _glitchEnded = false;

    private DG.Tweening.Sequence sequence;


    private void Start()
    {
        if (_glitchAni == null)
        {
            _glitchAni = FindAnyObjectByType<GlitchIndicator>().GetComponent<Animator>();
        }
        
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
        _glitchAni.Play("Glitch Count");
    }

    private void OnGlitchEnd()
    {
        _glitchEnded = true;
        _glitchAni.Play("Empty");
    }

}
