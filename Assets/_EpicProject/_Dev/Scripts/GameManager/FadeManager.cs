using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeManager : MonoBehaviour
{
    private FadeUI _fadeUI;

    private void Awake()
    {
        _fadeUI = FindAnyObjectByType<FadeUI>();
    }

    public void LoadScene(int index = -1)
    {
        StartCoroutine(LoadSceneCo(index));
    }

    private IEnumerator LoadSceneCo(int index)
    {
        yield return _fadeUI.FadeCo(0, 1);

        // index 지정되지 않으면 다음 씬으로 이동
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int targetIndex = (index == -1) ? currentIndex + 1 : index;
        
        if (targetIndex < SceneManager.sceneCountInBuildSettings)
        {
            // 씬 비동기 로드
            AsyncOperation loadOp = SceneManager.LoadSceneAsync(targetIndex);
            while (!loadOp.isDone)
            {
                yield return null;
            }
            
            yield return _fadeUI.FadeCo(1, 0, 0);
        }
        else
        {
            Debug.LogWarning(" 해당 인덱스의 씬이 존재하지 않습니다.");
        }
    }
}
