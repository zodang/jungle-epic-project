using Define;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSkinData", menuName = "Scriptable Objects/PlayerSkinData")]
public class PlayerSkinData : ScriptableObject
{
    public PlayerSkinType type;
    public AnimatorOverrideController overrideController;
}
