// FlagManager.cs
using System.Collections.Generic;
using UnityEngine;

public class FlagManager : MonoBehaviour
{
    private Dictionary<string, bool> flags = new Dictionary<string, bool>();
    // 만약 정수 값 플래그도 필요하다면:
    // private Dictionary<string, int> intFlags = new Dictionary<string, int>();

    /// <summary>
    /// 특정 이름의 플래그를 설정합니다.
    /// </summary>
    /// <param name="flagName">설정할 플래그의 이름 (고유해야 함)</param>
    /// <param name="value">설정할 값 (기본값: true)</param>
    public void SetFlag(string flagName, bool value = true)
    {
        if (string.IsNullOrEmpty(flagName))
        {
            Debug.LogWarning("<FlagManager> Attempted to set a flag with a null or empty name.");
            return;
        }
        flags[flagName] = value;
        Debug.Log($"<FlagManager> Flag '{flagName}' set to {value.ToString()}.");
    }

    /// <summary>
    /// 특정 이름의 플래그가 설정되어 있는지 (true인지) 확인합니다.
    /// 설정되지 않았거나 false이면 false를 반환합니다.
    /// </summary>
    public bool IsFlagSet(string flagName)
    {
        if (string.IsNullOrEmpty(flagName)) return false;
        return flags.TryGetValue(flagName, out bool value) && value;
    }

    /// <summary>
    /// (테스트 또는 디버깅용) 특정 플래그를 초기화(제거)합니다.
    /// </summary>
    public void ClearFlag(string flagName)
    {
        if (flags.ContainsKey(flagName))
        {
            flags.Remove(flagName);
            Debug.Log($"<FlagManager> Flag '{flagName}' cleared.");
        }
    }

    // --- 게임 저장/로드 시 이 flags 딕셔너리를 저장하고 불러오는 로직이 필요합니다 ---
    // 예:
    // public Dictionary<string, bool> GetAllFlags() { return new Dictionary<string, bool>(flags); }
    // public void LoadFlags(Dictionary<string, bool> loadedFlags) { flags = new Dictionary<string, bool>(loadedFlags); }
}