using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickableOutline : MonoBehaviour
{
    private static readonly int _outlineProperty = Shader.PropertyToID("_OnOutline");
    private SpriteRenderer _visualRenderer;
    private MaterialPropertyBlock _mpb;
    private RaycastHit2D[] _hoverBuffer = new RaycastHit2D[10];

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
        //Collider2D hit = Physics2D.OverlapPoint(worldPos, LayerMask.GetMask("Clickable"));

        //bool nowHovered = hit != null && hit.transform == transform;

        //if (nowHovered != _isHovered)
        //{
        //    _isHovered = nowHovered;
        //    SetOutline(_isHovered);
        //}
        // [MOD: SMG 25 - 06 - 28] 마우스 클릭 시, ClickableMask와 ClickableMaskBypass를 구분 및 동작
        RaycastHit2D[] hits = Physics2D.RaycastAll(
                worldPos,
                Vector2.zero,
                float.PositiveInfinity,
                LayerMask.GetMask("Clickable"))
            .OrderBy(h => h.transform.position.z)
            .ToArray();
        bool nowHovered = false;
        bool isMaskBypass = false;
        for (int i = 0; i < hits.Length; i++)
        {
            Collider2D coll = hits[i].collider;
            
            IClickable clickable = coll.GetComponentInParent<IClickable>();
            if (!clickable.IsUnityNull())
            {
                //clickable.OnClicked();
                if(coll.transform == transform)
                {
                    nowHovered = true;
                    break;
                }
            }

            ClickableMask clickableMask = coll.GetComponent<ClickableMask>();
            ClickableMaskBypass clickableMaskBypass = coll.GetComponent<ClickableMaskBypass>();
            if (!clickableMaskBypass.IsUnityNull())
            {
                isMaskBypass = true;
            }
            else if (!clickableMask.IsUnityNull() && !isMaskBypass)
            {
                break;
            }
        }
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
