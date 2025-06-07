using Define;
using UnityEngine;

public class EngineController : MonoBehaviour
{
    public Clickable CurrentTarget { get; private set; }

    private EngineUIController _engineUIController;
    private EngineBlockController _engineBlockInspector;

    private void Awake()
    {
        _engineUIController = GetComponent<EngineUIController>();
        _engineBlockInspector = GetComponent<EngineBlockController>();
    }

    private void Start()
    {
        gameObject.AddComponent<DraggableUI>();
        gameObject.SetActive(false);
    }

    public void OpenInspector(Clickable target)
    {
        AudioManager.instance.playSfx(SfxType.Open);
        
        // Block 세팅
        CurrentTarget = target;
        _engineBlockInspector.AddBlock(target);
        
        // UI 세팅
        _engineUIController.SetProfile(target.GetProfile(), target);
        _engineUIController.SetUIPosition(target);
        
        gameObject.SetActive(true);
    }

    public void CloseInspector()
    {
        AudioManager.instance.playSfx(SfxType.Close);

        CurrentTarget = null;
        _engineUIController.ClearProfile();
        gameObject.SetActive(false);
    }

    public void RefreshSlot(Clickable target)
    {
        CurrentTarget = target;
        _engineBlockInspector.AddBlock(target);
    }
}
