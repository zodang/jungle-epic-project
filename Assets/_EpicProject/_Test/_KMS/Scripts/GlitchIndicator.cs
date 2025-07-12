using UnityEngine;


public class GlitchIndicator : MonoBehaviour
{
    [SerializeField] private Animator _glitchAni;

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
        _glitchAni.Play("ON Glitch", 0, 0f);
        _glitchAni.Play("Glitch Count", 1, 0f);
    }

    private void OnGlitchEnd()
    {
        _glitchAni.Play("Empty", 1, 0f);
    }

}
