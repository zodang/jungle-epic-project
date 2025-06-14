using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleSceneManager : MonoBehaviour
{
    public void StartNewGame()
    {
        // 임시 테스트 씬 연결
        SceneManager.LoadScene("AAStageScene_CDE");
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void Start()
    {
        GameManager.Instance.AudioManager.PlayBgm(true);
    }
}
