using Define;
using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class FadeManager : MonoBehaviour
{
    public bool IsLoading { get; private set; }

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

    public void LoadNextScene(TransitionType transitionType = TransitionType.LoadingType)
    {
        // 다음 Scene으로 이동
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        StageManager.Instance.SaveSceneIndex(currentIndex);

        StartCoroutine(LoadSceneCo(currentIndex + 1, transitionType));
    }

    private IEnumerator LoadSceneCo(int index, TransitionType transitionType = TransitionType.LoadingType)
    {
        IsLoading = true;

        FindAnyObjectByType<PlayableDirector>()?.Stop();
        GameManager.Instance.TimeScaleManager.ResumeGame();

        int targetIndex = Mathf.Min(index, SceneManager.sceneCountInBuildSettings - 1);
        if (targetIndex >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogWarning(" 해당 인덱스의 씬이 존재하지 않습니다.");
            yield break;
        }

        yield return _transitionManager.TurnOnAni(transitionType);
            
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(targetIndex);
        while (!loadOp.isDone)
        {
            yield return null;
        }
        
        IsLoading = false;
        GameManager.Instance.AudioManager.FadeInAudio(0f);

        yield return _transitionManager.TurnOffAni();
    }
}
