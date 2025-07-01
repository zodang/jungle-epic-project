using Define;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    
    [SerializeField] private PlayerSkinData[] playerSkinData;
    private Dictionary<PlayerSkinType, PlayerSkinData> _skinDictionary = new();
    private RuntimeAnimatorController _defaultController;

    private bool _isAnimationActive;
    
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _defaultController = _animator.runtimeAnimatorController;
        
        // SkinDictionary 초기화
        foreach (var skin in playerSkinData)
        {
            if (!_skinDictionary.ContainsKey(skin.type))
            {
                _skinDictionary[skin.type] = skin;
            }
        }
    }
    
    public void ChangeSkin(PlayerSkinType type)
    {
        // 기본 Controller로 변경
        if (type == PlayerSkinType.Default)
        {
            _animator.runtimeAnimatorController = _defaultController;
            return;
        }
        
        // OverrideController로 변경
        if (_skinDictionary.TryGetValue(type, out PlayerSkinData skinData) && skinData.overrideController != null)
        {
            _animator.runtimeAnimatorController = skinData.overrideController;
            return;
        }
        
        Debug.LogWarning($"{type}의 스킨 없음!");
    }

    public void ActivateAnimation(bool isActive)
    {
        // 애니메이션 활성화, 비활성화 기능
        _isAnimationActive = isActive;
        _spriteRenderer.flipX = false;
    }

    private void Update()
    {
        if (!_isAnimationActive) return;
     
        Vector2 move = StageManager.Instance.InputManager.MoveInput;
        bool isMoving = move.sqrMagnitude > 0.01f;
        
        if (Mathf.Abs(move.x) > Mathf.Abs(move.y))
        {
            // 수평 우선
            move.y = 0;
        }
        else
        {
            // 수직 우선
            move.x = 0;
        }

        _animator.SetBool("IsMoving", isMoving);
        _animator.SetFloat("AbsMoveX", Mathf.Abs(move.x));
        _animator.SetFloat("MoveX", move.x);
        _animator.SetFloat("MoveY", move.y);

        // 좌우 반전
        if (Mathf.Abs(move.x) > 0.01f)
        {
            _spriteRenderer.flipX = move.x > 0;
        }
    }
}
