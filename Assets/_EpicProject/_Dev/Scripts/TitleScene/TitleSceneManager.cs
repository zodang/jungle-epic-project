using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleSceneManager : MonoBehaviour
{
    [SerializeField] private StageManager stageManagerPrefab;

    private void Awake()
    {
        if (stageManagerPrefab == null)
        {
            stageManagerPrefab = Resources.Load<StageManager>("Prefabs/StageManager");
        }
    }
    
    public void Start()
    {
        // GameManager.Instance.AudioManager.PlayBgm(true);
    }

    public void StartNewGame()
    {
        Instantiate(stageManagerPrefab, Vector3.zero, Quaternion.identity);
        GameManager.Instance.FadeManager.LoadScene(2);

        // 테스트 씬 연결
        //SceneManager.LoadScene("AAStageScene_CDE");
    }

    public void ContinueGame()
    {
        Instantiate(stageManagerPrefab, Vector3.zero, Quaternion.identity);
        // TODO: 클리어한 다음 씬 실행
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
