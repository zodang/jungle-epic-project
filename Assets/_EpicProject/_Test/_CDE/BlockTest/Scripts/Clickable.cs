using System.Collections.Generic;
using Define;
using UnityEngine;


public class Clickable : MonoBehaviour, IClickable
{
    // 저장할 Profile 데이터
    public string ID;
    private ClickableProfile _profile;
    
    public List<BlockType> BlockTypeList { get; private set; } = new List<BlockType>();
    [SerializeField] private List<BlockType> defaultBlockTypes = new List<BlockType>();

    private EngineController _engineController;

    private void Awake()
    {
        _engineController = FindAnyObjectByType<EngineController>();
    }

    public void Init(ClickableProfile profile)
    {
        _profile = profile;
    }

    private void Start()
    {
        // Default Block 추가
        foreach (var type in defaultBlockTypes)
        {
            if (!BlockTypeList.Contains(type))
            {
                BlockTypeList.Add(type);
            }
        }
        BlockManager.ApplyBlockToTarget(this, FindAnyObjectByType<BlockFactory>());
    }

    public void OnClicked()
    {
        // 클릭 시 InspectorUI 활성화
        _engineController.OpenInspector(this);
    }
    
    public void AddBlock(BlockType type)
    {
        // Inventory에서 Inspector에서 드롭 시
        BlockTypeList.Add(type);
        _engineController.RefreshSlot(this);
    }
    
    public void RemoveBlock(BlockType type)
    {
        // Inspector에서 Inventory로 드롭 시
        BlockTypeList.Remove(type);
        _engineController.RefreshSlot(this);
    }

    public ClickableProfile GetProfile()
    {
        return _profile;
    }

    public void UpdateNote(string newNote)
    {
        _profile.note = newNote;
    }
}
