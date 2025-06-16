using Define;
using UnityEngine;

public class GrassAnimation : MonoBehaviour
{
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        _animator.SetTrigger("StepTrigger");
        GameManager.Instance.AudioManager.PlaySfx(SfxType.Grass); // 잔디 밟는 소리 재생
    }
}
