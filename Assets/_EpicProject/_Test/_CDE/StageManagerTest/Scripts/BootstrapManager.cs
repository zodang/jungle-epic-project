using UnityEngine;

public class BootstrapManager : MonoBehaviour
{
    private void Awake()
    {
        SpawnMissingManager();
    }

    private void SpawnMissingManager()
    {
        // GameManager 없으면 생성
        if (GameManager.Instance == null)
        {
            GameManager gameManager = Resources.Load<GameManager>("Prefabs/GameManager");
            Instantiate(gameManager, Vector3.zero, Quaternion.identity);
            
        }

        // StageManager 없으면 생성
        if (StageManager.Instance == null)
        {
            StageManager stageManager = Resources.Load<StageManager>("Prefabs/StageManager");
            Instantiate(stageManager, Vector3.zero, Quaternion.identity);
        }
    }
}
