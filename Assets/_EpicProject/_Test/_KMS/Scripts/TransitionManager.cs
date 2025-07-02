using UnityEngine;
using System.Collections;

public class TransitionManager : MonoBehaviour
{
    private Animator _animator;
    private bool _isPlaying = false;  // 다시 실행하려면 false로 초기화 필요

    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    public IEnumerator TurnOnAni()
    {
        //_isPlaying = true;
        _animator.Play("Turn On");
        yield return new WaitForSeconds(2f);
    } 

    public IEnumerator TurnOffAni()
    {
        _animator.Play("Turn Off");
        yield return new WaitForSeconds(2f);
    }
}
