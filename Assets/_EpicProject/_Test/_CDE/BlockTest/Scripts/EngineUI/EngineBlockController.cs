using System.Collections.Generic;
using UnityEngine;

public class EngineBlockController : MonoBehaviour
{
    private BlockFactory _blockFactory;

    private EngineSlotGroup _engineSlotGroup;
    private List<InspectorSlot> _slotList;
    private Transform[] _slotTransforms;

    private void Awake()
    {
        _blockFactory = FindAnyObjectByType<BlockFactory>();
        _engineSlotGroup = FindAnyObjectByType<EngineSlotGroup>();
        
        _slotList = new List<InspectorSlot>(_engineSlotGroup.GetComponentsInChildren<InspectorSlot>());
        _slotTransforms = new Transform[_slotList.Count];
        for (int i = 0; i < _slotList.Count; i++)
        {
            _slotTransforms[i] = _slotList[i].transform;
        }
    }

    public void RemoveBlock()
    {
        // Inspector Slot의 기존 블록 제거
        foreach (var slot in _slotList)
        {
            var existing = slot.GetChildBlock();
            if (existing != null)
            {
                Destroy(existing.gameObject);
                slot.OnBlockRemoved();
            }
        }
    }
    
    public void AddBlock(Clickable target)
    {
        // Inspector Slot에 새 Block 추가
        RemoveBlock();
        BlockManager.ApplyBlockToTarget(target, _blockFactory, _slotTransforms);
    }
    
}
