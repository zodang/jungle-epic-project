using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeManager : MonoBehaviour
{
    private TransitionManager _transitionManager;

    private void Awake()
    {
        _transitionManager = FindAnyObjectByType<TransitionManager>();
    }

    public void LoadScene(int index)
    {
        // 특정 Scene으로 이동
        StartCoroutine(LoadSceneCo(index));
    }

    public void LoadCurrentScene()
    {
        // 현재 Scene으로 이동
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        StartCoroutine(LoadSceneCo(currentIndex));
    }

    public void LoadNextScene()
    {
        // 다음 Scene으로 이동
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        StageManager.Instance.SaveSceneIndex(currentIndex);

        StartCoroutine(LoadSceneCo(currentIndex + 1));
    }

    private IEnumerator LoadSceneCo(int index)
    {
        // 마지막 씬을 넘어갈 시 마지막 index 씬 호출
        int targetIndex = Mathf.Min(index, SceneManager.sceneCountInBuildSettings - 1);
        
        yield return _transitionManager.TurnOnAni();

        if (targetIndex < SceneManager.sceneCountInBuildSettings)
        {
            // 씬 비동기 로드
            AsyncOperation loadOp = SceneManager.LoadSceneAsync(targetIndex);
            while (!loadOp.isDone)
            {
                yield return null;
            }
            
            yield return _transitionManager.TurnOffAni();
        }
        else
        {
            Debug.LogWarning(" 해당 인덱스의 씬이 존재하지 않습니다.");
        }
    }
}
