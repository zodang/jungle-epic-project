using Define;
using Unity.VisualScripting;
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
        //Movement2D movement;
        if (!other.CompareTag("Player")) return;
        //movement = other.GetComponentInParent<Movement2D>();
        //if (movement.IsUnityNull()) return;
        _animator.SetTrigger("StepTrigger");
        GameManager.Instance.AudioManager.PlaySfx(SfxType.Grass); // 잔디 밟는 소리 재생
    }
}
