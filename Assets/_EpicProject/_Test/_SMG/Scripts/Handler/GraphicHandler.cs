using System.Collections.Generic;
using UnityEngine;

public class GraphicHandler : MonoBehaviour, IGraphicChangeable
{
    private List<Sprite> _graphics;
    private SpriteRenderer _spriteRenderer;

    public void Init(List<Sprite> graphics)
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _graphics = graphics;
    }
    
    public void SetGraphic(int index)
    {
        _spriteRenderer.sprite = _graphics[index];
    }
}
