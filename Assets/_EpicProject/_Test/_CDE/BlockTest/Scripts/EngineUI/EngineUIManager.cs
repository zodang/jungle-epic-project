using UnityEngine;
using DG.Tweening;

public class EngineUIManager : MonoBehaviour
{
    [SerializeField] private RectTransform leftGroup;
    [SerializeField] private RectTransform rightGroup;

    private Sequence _sequence;
    private float _outsidePos = -300;
    private float _insidePos = 10;
    private float _duration = 0.4f;

    private void Start()
    {
        _sequence = DOTween.Sequence();
        leftGroup.gameObject.SetActive(false);
        rightGroup.gameObject.SetActive(false);
    }

    public void ActivateTabHomeGroup(bool isActive)
    {
        float endPos = isActive ? _insidePos : _outsidePos;

        _sequence = DOTween.Sequence().SetAutoKill(false);

        _sequence.Append(leftGroup.DOAnchorPosX(endPos, _duration)).SetEase(Ease.InQuad)
            .Join(rightGroup.DOAnchorPosX(-endPos, _duration)).SetEase(Ease.InQuad);
    }
}
