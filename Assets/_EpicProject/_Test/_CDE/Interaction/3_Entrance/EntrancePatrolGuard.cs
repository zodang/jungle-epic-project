using Define;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GraphicSpriteData
{
    public GraphicType GraphicType;
    public List<Sprite> SpriteList;
}

public class EntrancePatrolGuard : MonoBehaviour, IFeatureResetable, IControllable
{
    // IControllable
    private Rigidbody2D _rigidbody2D;
    private Movement2D _movement2D;
    private bool _enableMove;
    
    // IGraphicChangeable
    private GraphicHandler _graphicHandler;
    [SerializeField] private GraphicSpriteData[] _graphicSpriteData;
    
    private PlayerAnimation _animation;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private int _currentGraphicType = 1;
    
    private void Awake()
    {
        ComponentHelper.TryGetOrAddComponent<GraphicHandler>(ref _graphicHandler, gameObject);
        _graphicHandler.OnSetValue += SetGraphic;
        
        _movement2D = GetComponent<Movement2D>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _rigidbody2D.gravityScale = 0;

        _animation = GetComponentInChildren<PlayerAnimation>();
        _animator = _animation.GetComponent<Animator>();
        _spriteRenderer = _animation.GetComponent<SpriteRenderer>();
        
        DisableControl();
        ResetFeature();
    }

    #region FeatureSetting

    private void Update()
    {
        if (!_enableMove) return;

        _movement2D.MoveDir = StageManager.Instance.InputManager.MoveInput;
        SetSpriteDirection(StageManager.Instance.InputManager.MoveInput);
    }

    public void EnableControl()
    {
        _enableMove = true;
        _rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        _movement2D.MoveDir = Vector3.zero;
        _animation.ActivateAnimation(true);
        
        SetGraphic(_currentGraphicType);
    }

    public void DisableControl()
    {
        _enableMove = false;
        _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
        _movement2D.MoveDir = Vector3.zero;
        _animation.ActivateAnimation(false);
        
        SetGraphic(_currentGraphicType);
    }

    private void SetGraphic(int index)
    {
        _currentGraphicType = index;
        
        _animation.ActivateAnimation(index == 1 && _enableMove);
        _animator.enabled = (index == 1);
        _spriteRenderer.flipX = false;
    }

    private void SetSpriteDirection(Vector2 dir)
    {
        if (_currentGraphicType != (int)GraphicType.High - 1) return;

        int idx = 1;
        
        if (Mathf.Abs(dir.y) >= Mathf.Abs(dir.x))
        {
            idx = dir.y > 0 ? 0 : 1; // Up : Down
        }
        else
        {
            idx = dir.x > 0 ? 2 : 3; // Right : Left
        }
        _spriteRenderer.sprite = _graphicSpriteData[0].SpriteList[idx];
    }
    
    public void ResetFeature()
    {
        // Todo: 리셋 기능
    }

    #endregion FeatureSetting
}
