// FlagTestInput.cs
using UnityEngine;

public class FlagTestInput : MonoBehaviour
{
    [Header("Test Flags (Match names used in NPCInteraction)")]
    [Tooltip("키 1을 누르면 설정/해제될 플래그 이름")]
    public string flag1Name = "npc1_Event_A"; // 예시 플래그 이름
    [Tooltip("키 2를 누르면 설정/해제될 플래그 이름")]
    public string flag2Name = "npcTest_F"; // 예시 플래그 이름
    // 필요하다면 더 많은 플래그 테스트 변수 추가 가능

    void Update()
    {
        if (StageBaseManager.Instance.FlagManager == null)
        {
            // FlagManager가 없으면 아무것도 하지 않음 (또는 에러 메시지)
            if (Input.anyKeyDown) // 아무 키나 눌렸을 때 한 번만 경고
            {
                Debug.LogWarning("<FlagTestInput> StageBaseManager.Instance.FlagManager is not available.");
            }
            return;
        }

        // 숫자 키 1로 flag1Name 플래그 상태 토글 (설정/해제 반복)
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (!string.IsNullOrEmpty(flag1Name))
            {
                bool currentFlagState = StageBaseManager.Instance.FlagManager.IsFlagSet(flag1Name);
                bool nextFlagState = !currentFlagState; // 현재 상태의 반대로 설정
                StageBaseManager.Instance.FlagManager.SetFlag(flag2Name, nextFlagState);
                Debug.Log($"<FlagTestInput> Toggled flag '{flag2Name}' to {nextFlagState.ToString()}");
            }
            else
            {
                Debug.LogWarning("<FlagTestInput> flag1Name is not set in Inspector.");
            }
        }

        // 숫자 키 2로 flag2Name 플래그 상태 토글
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (!string.IsNullOrEmpty(flag2Name))
            {
                bool currentFlagState = StageBaseManager.Instance.FlagManager.IsFlagSet(flag2Name);
                bool nextFlagState = !currentFlagState; // 현재 상태의 반대로 설정
                StageBaseManager.Instance.FlagManager.SetFlag(flag2Name, nextFlagState);
                Debug.Log($"<FlagTestInput> Toggled flag '{flag2Name}' to {nextFlagState.ToString()}");
            }
            else
            {
                Debug.LogWarning("<FlagTestInput> flag2Name is not set in Inspector.");
            }
        }

        // 숫자 키 0으로 모든 테스트 플래그 초기화 (선택 사항)
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            if (!string.IsNullOrEmpty(flag1Name)) StageBaseManager.Instance.FlagManager.ClearFlag(flag1Name);
            if (!string.IsNullOrEmpty(flag2Name)) StageBaseManager.Instance.FlagManager.ClearFlag(flag2Name);
            Debug.Log("<FlagTestInput> Cleared test flags (if set).");
        }

        // (선택 사항) 현재 플래그 상태를 GUI에 표시하여 쉽게 확인 (개발 중에만 사용)
        // DisplayCurrentFlagStates(); 
    }

    /*
    // (선택 사항) 테스트용 GUI 표시 함수
    private bool showFlagStatesGUI = true; // Inspector에서 켜고 끌 수 있게 public으로 해도 됨
    void OnGUI()
    {
        if (!showFlagStatesGUI || StageBaseManager.Instance.FlagManager == null) return;

        GUILayout.BeginArea(new Rect(10, Screen.height - 100, 300, 90)); // 화면 좌측 하단에 표시
        GUILayout.Label("--- Test Flag States ---");
        if (!string.IsNullOrEmpty(flag1Name))
        {
            GUILayout.Label($"Flag '{flag1Name}': {StageBaseManager.Instance.FlagManager.IsFlagSet(flag1Name)} (Press 1 to toggle)");
        }
        if (!string.IsNullOrEmpty(flag2Name))
        {
            GUILayout.Label($"Flag '{flag2Name}': {StageBaseManager.Instance.FlagManager.IsFlagSet(flag2Name)} (Press 2 to toggle)");
        }
        GUILayout.Label("(Press 0 to clear all test flags)");
        GUILayout.EndArea();
    }
    */
}