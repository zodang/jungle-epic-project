using System.Collections.Generic;
using Define;
using System.Linq;
using UnityEngine;

public class Inventory : BlockContainerBase
{
    public List<EngineBlock> BlockList = new List<EngineBlock>();
    public List<BlockType> DefaultBlockList = new();
    
    private InventoryUIController _inventoryUI;
    private Clickable _target;

    public bool StartGetPlayerControl = true;

    protected void Awake()
    {
        base.Awake();
        
        _inventoryUI = FindAnyObjectByType<InventoryUIController>();
        
        if (StartGetPlayerControl)
        {
            DefaultBlockList.Add(BlockType.PlayerControl);
        }
    }

    private void Start()
    {
        _target = StageBaseManager.Instance.PlayerManager.GetComponent<Clickable>();
        InventorySlot.SetInventory(this);
        InventorySlot.SetTargetClickable(_target);

        _inventoryUI.OnResetBtnClicked += ResetFeature;

        InitInventory();
    }

    private void OnDestroy()
    {
        _inventoryUI.OnResetBtnClicked -= ResetFeature;
    }

    private void CheckBlockDictionary()
    {
        var entries = BlockList.Select(b => b.name);
        string values = string.Join(", ", entries); 
        Debug.Log($"@@DE: {_target.name} : {values}");
    }
    
    public void AddBlock(EngineBlock block)
    {
        if (BlockList.Contains(block)) return;
        
        BlockList.Add(block);
        block.InitDefaultBlock(_target, InventorySlot);
        
        // CheckBlockDictionary();
    }

    protected override void RemoveBlock(int index)
    {
        if (index < 0 || index >= BlockList.Count) return;
        BlockList.RemoveAt(index);
        
        // CheckBlockDictionary();
    }

    public void Collect(BlockType type)
    {
        var factory = StageManager.Instance.BlockFactory;

        EngineBlock block = factory.CreateBlock(type, InventorySlot.transform); 
        RegisterBlockEvents(block);
        
        AddBlock(block);
    }

    private void InitInventory()
    {
        for (int i = 0; i < DefaultBlockList.Count; i++)
        {
            BlockType type = DefaultBlockList[i];
            Collect(type);
        }
    }
    
    private void ResetFeature()
    {
        if (_target == null) return;

        // Clickable의 기능 초기화
        IFeatureResetable resettable = _target.GetComponent<IFeatureResetable>();
        resettable?.ResetFeature();
        
        // Block의 UI 초기화
        EngineBlock[] blocks = _inventoryUI.GetComponentsInChildren<EngineBlock>(includeInactive: true);
        foreach (EngineBlock block in blocks)
        {
            block.ResetUI();
        }
    }
    
    private void Update()
    {
        for (int i = 1; i <= 4; i++)
        {
            KeyCode key = KeyCode.Alpha0 + i;
            int idx = i;

            if (Input.GetKeyDown(key))
            {
                if (InventorySlot.transform.childCount > idx)
                {
                    var block = InventorySlot.transform.GetChild(idx).GetComponent<EngineBlock>();
                    block?.RaiseEngineBlock();
                }
            }
            else if (Input.GetKeyUp(key))
            {
                if (InventorySlot.transform.childCount > idx)
                {
                    var block = InventorySlot.transform.GetChild(idx).GetComponent<EngineBlock>();
                    block?.DropEngineBlock();
                }
            }
        }
    }
}
