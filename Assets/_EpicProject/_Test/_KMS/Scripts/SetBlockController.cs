using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class SetBlockController : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        // EngineSlot.OnBlockPlaced += OnBlockAnimation;
    }
    void OnBlockAnimation()
    {
        if (_animator != null)
        {
            print("여기서 애니메이션 실행");
            _animator.Play("Set Block");
        }
        else
        {
            Debug.LogWarning("Animator is not assigned or missing.");
        }
    }
}
