using UnityEditor;
using UnityEngine;

public class TitleSceneManager : MonoBehaviour
{
    [SerializeField] private StageManager stageManagerPrefab;

    private void Awake()
    {
        if (stageManagerPrefab == null)
        {
            stageManagerPrefab = Resources.Load<StageManager>("Prefabs/StageManager");
        }
        
        // 테스트용 코드 추가
        gameObject.AddComponent<BootstrapManager>();
    }
    
    public void Start()
    {
        // GameManager.Instance.AudioManager.PlayBgm(true);
    }

    public void StartNewGame()
    {
        GameManager.Instance.SaveManager.DeleteStageData();
        Instantiate(stageManagerPrefab, Vector3.zero, Quaternion.identity);
        GameManager.Instance.FadeManager.LoadScene(2);
    }

    public void ContinueGame()
    {
        Instantiate(stageManagerPrefab, Vector3.zero, Quaternion.identity);
        int clearStageIndex = GameManager.Instance.SaveManager.LoadStageData().ClearStageIndex;
        int targetIndex = Mathf.Max(2, clearStageIndex + 1);
        
        GameManager.Instance.FadeManager.LoadScene(targetIndex);
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
