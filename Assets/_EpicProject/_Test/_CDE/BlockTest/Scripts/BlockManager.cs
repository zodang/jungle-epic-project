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
            FeatureBlock featureBlock = factory.CreateFeatureBlock(blockType, slot);

            // Slot이 있다면 해당 Slot에 배치
            if (slot != null && slot.TryGetComponent<InspectorSlot>(out InspectorSlot inspectorSlot))
            {
                inspectorSlot.OnBlockDrop(featureBlock);
            }
            
            // 기능 활성화
            Component feature = target.GetComponent(featureBlock.RequiredFeatureType);
            featureBlock.Activate(feature);
        }
    }

    public static void RemoveBlockFromTarget(Clickable target, Transform[] slotList)
    {
        foreach (var slot in slotList)
        {
            FeatureBlock featureBlock = slot.GetComponentInChildren<FeatureBlock>();
            
            // slot의 자식 오브젝트에 Block이 없다면 넘어간다.
            if (featureBlock == null) continue;
            
            // Slot이 있다면 해당 Slot에서 제거
            if (slot.TryGetComponent<InspectorSlot>(out var inspectorSlot))
            {
                inspectorSlot.OnBlockRemoved();
            }
            
            // 기능 비활성화
            Component feature = target.GetComponent(featureBlock.RequiredFeatureType);
            featureBlock.Deactivate(feature);
            
            // Block 삭제
            GameObject.Destroy(featureBlock.gameObject);
        }
    }
}
