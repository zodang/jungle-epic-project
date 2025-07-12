using UnityEngine;
using System.Collections;
using Define;

public class TransitionManager : MonoBehaviour
{
    private Animator _animator;
    private bool _isPlaying = false;  // 다시 실행하려면 false로 초기화 필요

    private string _onAnimation = "TurnOn";
    private string _offAnimation = "TurnOff";

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    public IEnumerator TurnOnAni(TransitionType type)
    {
        // Transition Type 변경
        ChangeAnimation(type);
        
        _animator.Play(_onAnimation);
        yield return new WaitForSeconds(2f);
    } 

    public IEnumerator TurnOffAni()
    {
        _animator.Play(_offAnimation);
        yield return new WaitForSeconds(2f);
    }

    private void ChangeAnimation(TransitionType type)
    {
        // Transition Animation Controller에서 state의 string으로 구분
        switch (type)
        {
            case TransitionType.FadeType:
                _onAnimation = "FadeOut";
                _offAnimation = "FadeIn";
                break;
            case TransitionType.LoadingType:
            default:
                _onAnimation = "TurnOn";
                _offAnimation = "TurnOff";
                break;
        }
    }
}
