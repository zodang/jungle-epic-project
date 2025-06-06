using Unity.VisualScripting;
using UnityEngine;

public class YSortOrder : MonoBehaviour
{
    private SpriteRenderer[] _spriteRenderers;
    private int _yPos;

    private void Awake()
    {
        _spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(_spriteRenderers.Length > 0)
        {
            _yPos = (int)(transform.position.y * 100f);
            for(int i = 0; i < _spriteRenderers.Length; i++)
            {
                _spriteRenderers[i].sortingOrder = -_yPos;
            }
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_spriteRenderers.Length <= 0) return;

        int newYPos = (int)(transform.position.y * 100f);
        if (_yPos == newYPos) return;

        _yPos = newYPos;
        for (int i = 0; i < _spriteRenderers.Length; i++)
        {
            _spriteRenderers[i].sortingOrder = -_yPos;
        }
    }
}
