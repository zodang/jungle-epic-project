using Define;
using UnityEditor;
using UnityEngine;

public class TitleSceneManager : MonoBehaviour
{
    [SerializeField] private StageManager stageManagerPrefab;
    private TitleSceneUIManager _uiManager;
    private int _clearStageIndex;

    private void Awake()
    {
        if (stageManagerPrefab == null)
        {
            stageManagerPrefab = Resources.Load<StageManager>("Prefabs/StageManager");
        }

        // 테스트용 코드 추가
        gameObject.AddComponent<BootstrapManager>();
        _uiManager = FindAnyObjectByType<TitleSceneUIManager>();
    }
    
    public void Start()
    {
       
        
        // 이어하기 버튼 활성화
        _clearStageIndex = GameManager.Instance.SaveManager.LoadStageData().ClearStageIndex;
        _uiManager.SetContinueBtn(_clearStageIndex > 2);
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
        int targetIndex = Mathf.Max(2, _clearStageIndex + 1);
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
