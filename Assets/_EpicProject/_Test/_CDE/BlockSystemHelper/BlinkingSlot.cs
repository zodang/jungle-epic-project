using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BlinkingSlot : MonoBehaviour
{
    private Image _image;
    private Tween _blinkTween;

    private void Awake()
    {
        _image = GetComponent<Image>();
    }

    public void StartBlinking()
    {
        _blinkTween = _image
            .DOFade(.3f, 0.5f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    public void StopBlinking()
    {
        _blinkTween?.Kill();
        var color = _image.color;
        color.a = 0f;
        _image.color = color;
    }
}