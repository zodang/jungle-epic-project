using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickableOutline : MonoBehaviour
{
    private static readonly int _outlineProperty = Shader.PropertyToID("_OnOutline");
    private SpriteRenderer _visualRenderer;
    private MaterialPropertyBlock _mpb;
    private RaycastHit2D[] _hoverBuffer = new RaycastHit2D[10];
    private ContactFilter2D _clickableFilter;

    private bool _isHovered = false;

    private void Awake()
    {
        _mpb = new MaterialPropertyBlock();
        _visualRenderer = GetComponent<SpriteRenderer>();
        
        // 클릭 위치 오브젝트 검출을 위해 Clickable 레이어만 검사
        int clickableLayerMask = LayerMask.GetMask("Clickable");
        _clickableFilter = new ContactFilter2D();
        _clickableFilter.SetLayerMask(clickableLayerMask);
        _clickableFilter.useTriggers = Physics2D.queriesHitTriggers;
    }

    private void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        // [MOD: SMG 25 - 06 - 28] 마우스 클릭 시, ClickableMask와 ClickableMaskBypass를 구분 및 동작
        int hitCount = GetSortedClickableHits(worldPos);
        bool nowHovered = false;
        bool isMaskBypass = false;
        bool isMask = false;
        ClickableMask mask = null;
        ClickableMaskSortOrder clickableMaskSortOrder = ClickableMaskSortOrder.ForePlayer;
        
        for (int i = 0; i < hitCount; i++)
        {
            Collider2D coll = _hoverBuffer[i].collider;

            ClickableMask clickableMask = coll.GetComponent<ClickableMask>();
            ClickableMaskBypass clickableMaskBypass = coll.GetComponent<ClickableMaskBypass>();
            if (!clickableMaskBypass.IsUnityNull())
            {
                isMaskBypass = true;
                continue;
            }
            else if (!clickableMask.IsUnityNull() && !isMaskBypass)
            {
                if(clickableMask.SortOrder == ClickableMaskSortOrder.ForePlayer)
                {
                    break;
                }
                else
                {
                    mask = clickableMask;
                    isMask = true;
                    clickableMaskSortOrder = clickableMask.SortOrder;
                    continue;
                }
            }

            IClickable clickable = coll.GetComponentInParent<IClickable>();
            if (!clickable.IsUnityNull())
            {
                if(coll.transform == transform)
                {
                    if(isMask)
                    {
                        if(clickableMaskSortOrder == ClickableMaskSortOrder.PlayerAndObject)
                        {
                            if(coll.transform.position.y >= mask.transform.position.y)
                            {
                                if (!coll.TryGetComponent<ClickableYAnchor>(out ClickableYAnchor clickableYAnchor) ||
                                    clickableYAnchor.YAnchor.IsUnityNull() ||
                                    clickableYAnchor.YAnchor.transform.position.y >= mask.transform.position.y)
                                {
                                    continue;
                                }
                            }
                        }
                        else if(clickableMaskSortOrder == ClickableMaskSortOrder.MidGround)
                        {
                            if(!coll.GetComponent<ClickableUnterTag>().IsUnityNull())
                            {
                                continue;
                            }
                        }
                    }
                    nowHovered = true;
                    break;
                }
            }
        }
        if (nowHovered != _isHovered)
        {
            _isHovered = nowHovered;
            SetOutline(_isHovered);
        }
    }
    
    public void SetOutline(bool active)
    {
        _visualRenderer.GetPropertyBlock(_mpb);
        _mpb.SetFloat(_outlineProperty, active ? 1 : 0);
        _visualRenderer.SetPropertyBlock(_mpb);
    }

    private int GetSortedClickableHits(Vector2 worldPos)
    {
        int hitCount;
        while (true)
        {
            hitCount = Physics2D.Raycast(
                worldPos,
                Vector2.zero,
                _clickableFilter,
                _hoverBuffer,
                float.PositiveInfinity);

            if (hitCount < _hoverBuffer.Length) break;
            
            Array.Resize(ref _hoverBuffer, _hoverBuffer.Length * 2);
        }

        // z 위치 기준으로 정렬
        Array.Sort(_hoverBuffer, 0, hitCount, RaycastHitZComparer.Instance);
        return hitCount;
    }

    private sealed class RaycastHitZComparer : IComparer<RaycastHit2D>
    {
        public static readonly RaycastHitZComparer Instance = new RaycastHitZComparer();

        public int Compare(RaycastHit2D x, RaycastHit2D y)
        {
            return x.collider.transform.position.z.CompareTo(y.collider.transform.position.z);
        }
    }
}
