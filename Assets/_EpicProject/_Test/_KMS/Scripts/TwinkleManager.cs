using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwinkleManager : MonoBehaviour
{
    [Header("Star Settings")]
    public List<GameObject> starPrefabs;
    public Transform spawnPoint;
    public float xRange = 1.0f;

    [Header("Timing Settings")]
    public float spawnInterval = 0.8f;
    public float moveSpeed = 1.0f;         // 별이 위로 올라가는 속도

    private void Start()
    {
        StartCoroutine(SpawnStarsLoop());
    }

    private IEnumerator SpawnStarsLoop()
    {
        while (true)
        {
            Vector3 spawnPos = new Vector3(
                spawnPoint.position.x + Random.Range(-xRange, xRange),
                spawnPoint.position.y,
                spawnPoint.position.z
            );

            int index = Random.Range(0, starPrefabs.Count);
            GameObject prefabToSpawn = starPrefabs[index];

            GameObject star = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity, transform);
            star.AddComponent<TwinkleMover>().Initialize(moveSpeed, spawnPoint.position.y + 5f);

            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
