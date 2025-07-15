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
    [SerializeField] private Animator _animator;

    private GraphicType _currentGraphicType = GraphicType.Middle;
    
    private void Awake()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        //_animator = GetComponentInChildren<Animator>();
        _isAnimated = _animator != null;
    }

    public void Init(GraphicType type)
    {
        _currentGraphicType = type;
    }

    public void SetValue(GraphicType type)
    {
        // Index 체크 후 Sprite 변경
        if ((int)type >= _btnGraphicSprites.Count || _btnGraphicSprites[(int)type] == null) return;
        
        _currentGraphicType = type;
        
        _spriteRenderer.sprite = _btnGraphicSprites[(int)type];
        _spriteRenderer.flipX = false;
        
        OnSetValue?.Invoke((int)type);

        // 애니메이션 관련 오브젝트 구분
        if (!_isAnimated) return;
        _animator.enabled = ((int)type == 1);
    }
    
    public GraphicType GetCurrentValue()
    {
        return _currentGraphicType;
    }
    
    public void SetSpriteDirection(Vector2 dir)
    {
        // 애니메이션 오브젝트 시 이동에 따른 Sprite 변경
        if (!_isAnimated) return;
        if (_currentGraphicType != GraphicType.High) return;

        int index = 1;
        
        if (Mathf.Abs(dir.y) >= Mathf.Abs(dir.x))
        {
            index = dir.y > 0 ? 0 : 1; // Up : Down
        }
        else
        {
            index = 2; // Right : Left
        }

        // Index 체크 후 Sprite 변경
        if (index >= highGraphicSprites.Count || highGraphicSprites[index] == null) return;
        _spriteRenderer.sprite = highGraphicSprites[index];
    }
}
