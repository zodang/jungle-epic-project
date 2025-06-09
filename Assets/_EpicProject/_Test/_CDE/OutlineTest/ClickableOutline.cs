using UnityEngine;
using UnityEngine.EventSystems;

public class ClickableOutline : MonoBehaviour
{
    private static readonly int _outlineProperty = Shader.PropertyToID("_OnOutline");
    private SpriteRenderer _visualRenderer;
    private MaterialPropertyBlock _mpb;

    private bool _isHovered = false;

    private void Awake()
    {
        _mpb = new MaterialPropertyBlock();
        _visualRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D hit = Physics2D.OverlapPoint(worldPos, LayerMask.GetMask("Clickable"));

        bool nowHovered = hit != null && hit.transform == transform;

        if (nowHovered != _isHovered)
        {
            _isHovered = nowHovered;
            SetOutline(_isHovered);
        }
    }

    private void SetOutline(bool active)
    {
        _visualRenderer.GetPropertyBlock(_mpb);
        _mpb.SetFloat(_outlineProperty, active ? 1 : 0);
        _visualRenderer.SetPropertyBlock(_mpb);
    }
}
