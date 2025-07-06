using Unity.VisualScripting;
using UnityEngine;

public class ShadowFollower : MonoBehaviour
{
    public Transform shadow;

    public Vector3 shadowOffset = Vector3.zero;
    public Vector3 shadowRotationOffset = Vector3.zero;
    public Vector3 shadowScaleMultiplier = Vector3.one;

    public float minScaleMultiplier = 1f; // 최대 거리에서의 최소 크기
    public float maxScaleMultiplier = 2f; // 최소 거리에서의 최대 크기

    public float _minDist = 1f;
    public float _maxDist = 6f;


    void Update()
    {
        if (shadow.IsUnityNull()) return;

        Vector3 shadowPos = shadow.position;
        shadowPos.x = transform.position.x + shadowOffset.x;
        shadow.position = shadowPos;

        shadow.rotation = Quaternion.Euler(0, 0, transform.eulerAngles.z + shadowRotationOffset.z);

        float distanceY = Mathf.Abs(transform.position.y - shadow.position.y);
        Debug.Log("distanceY: " + distanceY);
        float scaleMultiplier = CalculateScaleMultiplier(distanceY);

        shadow.localScale = Vector3.Scale(transform.localScale, shadowScaleMultiplier * scaleMultiplier);
        shadow.localScale = new Vector3(shadow.localScale.x, shadow.localScale.y, 1f);
    }

    float CalculateScaleMultiplier(float distanceY)
    {
        float t = Mathf.InverseLerp(_minDist, _maxDist, distanceY);
        return Mathf.Lerp(minScaleMultiplier, maxScaleMultiplier, t);
    }
}
