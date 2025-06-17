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
            _slotList[i].SetSlotIndex(i);
        }
    }
    
    public void AddBlock(Clickable target)
    {
        for (int i = 0; i < _slotList.Count; i++)
        {
            var slot = _slotList[i];

            bool shouldExist = target.SlotBlockMap.TryGetValue(i, out var expectedType);
            var currentType = slot.CurrentBlockType;

            if (!shouldExist && currentType != null)
            {
                var feature = target.GetComponent(slot.CurrentBlock.RequiredFeatureType);
                slot.CurrentBlock.Deactivate(feature);
                slot.ClearBlock();
            }
            
            else if (shouldExist && currentType != expectedType)
            {
                if (slot.CurrentBlock != null)
                {
                    var feature = target.GetComponent(slot.CurrentBlock.RequiredFeatureType);
                    slot.CurrentBlock.Deactivate(feature);
                    slot.ClearBlock();
                }

                var block = StageManager.Instance.BlockFactory.CreateFeatureBlock(expectedType, slot.transform);
                var f = target.GetComponent(block.RequiredFeatureType);
                block.Activate(f);
                slot.SetBlock(block);
            }
        }
    }
}
