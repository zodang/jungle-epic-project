using System.Collections.Generic;
using UnityEngine;

public class PinwheelWindZone : MonoBehaviour
{
    private readonly List<Animator> _animators = new List<Animator>();
    private readonly List<SpriteRenderer> _spriteRenderers = new List<SpriteRenderer>();

    private void Awake()
    {
        gameObject.GetComponentsInChildren(true, _animators);
        gameObject.GetComponentsInChildren(true, _spriteRenderers);
    }

    public void ApplyAnimator(float fanPower, float lv1Threshold)
    {
        float animatorSpeed = Mathf.Clamp(fanPower / 10f, 0.5f, 3f);
        for (int i = 0; i < _animators.Count; i++)
        {
            _animators[i].speed = animatorSpeed;
        }
    }

    public void ApplyAlpha(float fanPower, float lv1Threshold)
    {
        float alpha = Mathf.Clamp01(fanPower / (lv1Threshold * 2f));
        for (int i = 0; i < _spriteRenderers.Count; i++)
        {
            Color color = _spriteRenderers[i].color;
            color.a = alpha;
            _spriteRenderers[i].color = color;
        }
    }
}
