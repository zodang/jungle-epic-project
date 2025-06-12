using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void OnStartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        Debug.Log("게임시작");
    }

    // 환경 설정 버튼 클릭
    public void OnOpenSettings()
    {
        // TODO: 앞으로 만들게 될 설정창
        Debug.Log("환경설정");
    }

    // 게임 종료 버튼 클릭
    public void OnQuitGame()
    {
        // TODO: 게임 끄기
        Debug.Log("게임끄기");
    }
}
