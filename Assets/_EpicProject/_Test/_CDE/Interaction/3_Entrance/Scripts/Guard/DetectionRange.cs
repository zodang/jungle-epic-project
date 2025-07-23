using System;
using UnityEngine;

public class DetectionRange : MonoBehaviour
{
    public Action<GameObject> OnPlayerDetected;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    private Collider2D _collider;

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (_spriteRenderer == null) return;
        float absX = Mathf.Abs(_collider.offset.x);
        _collider.offset = new Vector2(_spriteRenderer.flipX ? absX : -absX, _collider.offset.y);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (StageBaseManager.Instance.DialogueManager.IsDialogueActive()) return;
        
        OnPlayerDetected?.Invoke(other.gameObject);
    }

    private void OnDestroy()
    {
        OnPlayerDetected = null;
    }
}
