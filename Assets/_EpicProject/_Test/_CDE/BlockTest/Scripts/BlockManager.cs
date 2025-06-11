using UnityEngine;

public class BlockManager
{
    public static void ApplyBlockToTarget(Clickable target, BlockFactory factory, Transform[] slotList = null)
    {
        var blockTypes = target.BlockTypeList;

        for (int i = 0; i < blockTypes.Count; i++)
        {
            var blockType = blockTypes[i];

            // Block 생성
            bool hasSlot = slotList != null && i < slotList.Length;
            Transform slot = hasSlot ? slotList[i] : null;
            EngineBlock engineBlock = factory.CreateFeatureBlock(blockType, slot);

            // Slot이 있다면 해당 Slot에 배치
            if (slot != null && slot.TryGetComponent<EngineSlot>(out EngineSlot inspectorSlot))
            {
                inspectorSlot.OnBlockDrop(engineBlock);
            }
            
            // 기능 활성화
            Component feature = target.GetComponent(engineBlock.RequiredFeatureType);
            engineBlock.Activate(feature);
        }
    }

    public static void RemoveBlockFromTarget(Clickable target, Transform[] slotList)
    {
        foreach (var slot in slotList)
        {
            EngineBlock engineBlock = slot.GetComponentInChildren<EngineBlock>();
            
            // slot의 자식 오브젝트에 Block이 없다면 넘어간다.
            if (engineBlock == null) continue;
            
            // Slot이 있다면 해당 Slot에서 제거
            if (slot.TryGetComponent<EngineSlot>(out var inspectorSlot))
            {
                inspectorSlot.OnBlockRemoved();
            }
            
            // 기능 비활성화
            Component feature = target.GetComponent(engineBlock.RequiredFeatureType);
            engineBlock.Deactivate(feature);
            
            // Block 삭제
            GameObject.Destroy(engineBlock.gameObject);
        }
    }
}
