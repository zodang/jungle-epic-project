using UnityEngine;

public class SpeechAnchorMover : MonoBehaviour
{
    // 크기 조정의 기준 오브젝트
    [SerializeField] private Transform targetObj;

    // 오브젝트의 크기 변경 범위
    [SerializeField] private float minScale = 0.5f;
    [SerializeField] private float maxScale = 2.0f;

    [Header("최소/최대 크기일 때 offset")]
    [SerializeField] private float offsetAtMinScale = 3.3f;
    [SerializeField] private float offsetAtMaxScale = 1.7f;

    void LateUpdate()
    {
        float scaleY = targetObj ? targetObj.lossyScale.y : 1f;
        float t = Mathf.InverseLerp(minScale, maxScale, scaleY);
        float yOffset = Mathf.Lerp(offsetAtMinScale, offsetAtMaxScale, t);
        transform.localPosition = new Vector3(0, yOffset, 0);
    }
}
