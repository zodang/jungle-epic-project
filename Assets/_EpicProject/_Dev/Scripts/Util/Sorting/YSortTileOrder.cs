using UnityEngine;
using UnityEngine.Tilemaps;

public class YSortTileOrder : MonoBehaviour
{
    [Tooltip("숫자가 높을수록 같은 Y위치에 있을 때 더 앞에 보입니다.")]
    public int sortingOrderBias = 0;

    // --- [새로 추가된 옵션] ---
    [Tooltip("이 옵션을 켜면 Y위치와 상관없이 레이어의 맨 뒤로 보냅니다.")]
    public bool forceToBottom = false;

    private TilemapRenderer[] _tilemapRenderers;
    private int _yPos;

    // OnEnable, Awake는 이전과 동일
    private void OnEnable()
    {
        _yPos = int.MinValue;
    }

    private void Awake()
    {
        _tilemapRenderers = GetComponentsInChildren<TilemapRenderer>();
    }

    // Start 함수는 이제 필요 없습니다.

    void LateUpdate()
    {
        if (_tilemapRenderers.Length <= 0) return;

        // --- [수정된 핵심 로직] ---
        if (forceToBottom)
        {
            // '강제 맨 뒤' 옵션이 켜져 있다면, Order in Layer를 아주 낮은 값으로 고정합니다.
            for (int i = 0; i < _tilemapRenderers.Length; i++)
            {
                _tilemapRenderers[i].sortingOrder = -30000;
            }
        }
        else
        {
            // 옵션이 꺼져 있다면, 원래의 Y-Sort 로직을 수행합니다.
            int newYPos = (int)(transform.position.y * 100f);
            if (_yPos == newYPos) return;

            _yPos = newYPos;
            for (int i = 0; i < _tilemapRenderers.Length; i++)
            {
                _tilemapRenderers[i].sortingOrder = -_yPos + sortingOrderBias;
            }
        }
        // --- [수정 끝] ---
    }
}
