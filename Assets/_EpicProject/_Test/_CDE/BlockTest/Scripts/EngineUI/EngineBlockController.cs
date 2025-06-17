using System.Collections.Generic;
using UnityEngine;

public class EngineBlockController : MonoBehaviour
{
    private EngineSlotGroup _engineSlotGroup;
    private List<EngineSlot> _slotList;
    private Transform[] _slotTransforms;

    private void Awake()
    {
        _engineSlotGroup = GetComponentInChildren<EngineSlotGroup>();
        
        // 실제 Slot의 첫번재 Index가 UI상 마지막 Index로 사용
        _slotList = new List<EngineSlot>(_engineSlotGroup.GetComponentsInChildren<EngineSlot>());
        _slotList.Reverse(); 
        
        _slotTransforms = new Transform[_slotList.Count];
        for (int i = 0; i < _slotList.Count; i++)
        {
            _slotTransforms[i] = _slotList[i].transform;
        }
    }

    private void RemoveBlockFromSlot()
    {
        // Engine Slot의 기존 블록 제거
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
        RemoveBlockFromSlot();
        BlockManager.ApplyBlockToTarget(target, StageManager.Instance.BlockFactory, _slotTransforms);
    }
    
}
