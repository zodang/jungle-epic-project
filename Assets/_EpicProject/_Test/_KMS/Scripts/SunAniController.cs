using UnityEngine;
using System.Collections;

public class AnimationSpawner : MonoBehaviour
{
    [Header("생성할 애니메이션 프리팹")]
    public GameObject animationPrefab;

    [Header("애니 실행 중 숨길 이미지")]
    public SpriteRenderer SunImage;

    [Header("프리팹이 생성될 위치 (없으면 이 오브젝트 위치)")]
    public Transform spawnPoint;

    [Header("생성 간격 (초)")]
    public float interval = 5f;

    void Start()
    {
        if (spawnPoint == null)
            spawnPoint = transform;

        // 처음에는 이미지 보이게
        if (SunImage != null)
            SunImage.enabled = true;

        InvokeRepeating(nameof(PlayAnimation), 0f, interval);
    }

    void PlayAnimation()
    {
        // 1) 애니 실행 직전 SunImage 숨기기
        if (SunImage != null)
            SunImage.enabled = false;

        // 2) 프리팹 인스턴스화
        GameObject go = Instantiate(
            animationPrefab,
            spawnPoint.position,
            spawnPoint.rotation
        );

        // 3) 5초 뒤에 애니 파편(go) 파괴 & 이미지 복원
        Destroy(go, 0.3f);
        StartCoroutine(RestoreSunImageAfterDelay(0.3f));
    }

    IEnumerator RestoreSunImageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (SunImage != null)
            SunImage.enabled = true;
    }

    // 필요하다면 생성 멈추기
    public void StopSpawning()
    {
        CancelInvoke(nameof(PlayAnimation));
    }
}
