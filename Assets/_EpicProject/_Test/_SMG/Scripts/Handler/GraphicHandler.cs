using System;
using System.Collections.Generic;
using UnityEngine;

public class GraphicHandler : MonoBehaviour, IGraphicChangeable
{
    public Action<int> OnSetValue;
    
    [SerializeField] private List<Sprite> _graphics;
    private SpriteRenderer _spriteRenderer;
    
    private void Awake()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public void InitGraphics (List<Sprite> graphics)
    {
        _graphics = graphics;
    }
    
    public void SetGraphic(int index)
    {
        _spriteRenderer.sprite = _graphics[index];
        OnSetValue?.Invoke(index);
    }
}
