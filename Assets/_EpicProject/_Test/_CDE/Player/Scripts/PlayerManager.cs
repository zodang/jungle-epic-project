using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public Inventory Inventory { get; private set; }
    public PlayerInteraction Interaction { get; private set; }
    public PlayerFeature Feature { get; private set; }
    public PlayerAnimation Animation { get; private set; }

    private void Awake()
    {
        Inventory = GetComponent<Inventory>();
        Interaction = GetComponent<PlayerInteraction>();
        Feature = GetComponent<PlayerFeature>();
        Animation = GetComponentInChildren<PlayerAnimation>();
    }

    private void Start()
    {
        Feature.OnControlEnabled += Animation.ActivateAnimation;
        Feature.OnPlayerTwinkled += Animation.ChangeSkin;
    }
}
