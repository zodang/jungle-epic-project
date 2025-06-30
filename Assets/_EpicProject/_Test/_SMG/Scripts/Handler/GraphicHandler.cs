using Define;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GraphicHandler : MonoBehaviour, IGraphicChangeable
{
    public Action<int> OnSetValue;

    // Graphic Button 클릭 시 변경할 Sprites
    [SerializeField] private List<Sprite> _btnGraphicSprites;
    // High Graphic에서 사용할 sprites
    [SerializeField] private List<Sprite> highGraphicSprites;

    private bool _isAnimated;
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;

    private int _currentGraphicType = 1;
    
    private void Awake()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _animator = GetComponentInChildren<Animator>();
        _isAnimated = _animator != null;
    }

    public void InitGraphics (List<Sprite> graphics)
    {
        _btnGraphicSprites = graphics;
    }
    
    public void SetGraphic(int index)
    {
        // Index 체크 후 Sprite 변경
        if (index >= _btnGraphicSprites.Count || _btnGraphicSprites[index] == null) return;
        
        _currentGraphicType = index;
        
        _spriteRenderer.sprite = _btnGraphicSprites[index];
        _spriteRenderer.flipX = false;
        
        OnSetValue?.Invoke(index);

        // 애니메이션 관련 오브젝트 구분
        if (!_isAnimated) return;
        _animator.enabled = (index == 1);
    }
    
    public void SetSpriteDirection(Vector2 dir)
    {
        // 애니메이션 오브젝트 시 이동에 따른 Sprite 변경
        if (!_isAnimated) return;
        if (_currentGraphicType != (int)GraphicType.High - 1) return;

        int idx = 1;
        
        if (Mathf.Abs(dir.y) >= Mathf.Abs(dir.x))
        {
            idx = dir.y > 0 ? 0 : 1; // Up : Down
        }
        else
        {
            idx = 2; // Right : Left
        }

        // Index 체크 후 Sprite 변경
        if (idx >= highGraphicSprites.Count || highGraphicSprites[idx] != null) return;
        _spriteRenderer.sprite = highGraphicSprites[idx];
    }
}
