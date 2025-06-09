using UnityEngine;

[RequireComponent(typeof(Clickable))]
public class ClickableOutline : MonoBehaviour
{
    private static readonly int _outlineProperty = Shader.PropertyToID("_OnOutline");
    private SpriteRenderer _visualRenderer;
    private MaterialPropertyBlock _mpb;

    private void Awake()
    {
        _mpb = new MaterialPropertyBlock();
        _visualRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnMouseEnter()
    {
        if (_visualRenderer == null) return; 

        // Outline 활성화
        _visualRenderer.GetPropertyBlock(_mpb);
        _mpb.SetFloat(_outlineProperty, 1);
        _visualRenderer.SetPropertyBlock(_mpb);
    }

    private void OnMouseExit()
    {
        if (_visualRenderer == null) return; 

        // Outline 비활성화
        _visualRenderer.GetPropertyBlock(_mpb);
        _mpb.SetFloat(_outlineProperty, 0);
        _visualRenderer.SetPropertyBlock(_mpb);
    }
}
