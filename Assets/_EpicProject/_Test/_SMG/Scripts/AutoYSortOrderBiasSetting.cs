using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class AutoYSortOrderBiasSetting : MonoBehaviour
{
    float minRange;
    float maxRange;

    [ContextMenu("Excute")]
    public void Excute()
    {
        Dictionary<float, List<YSortOrder>> dictionary = new Dictionary<float, List<YSortOrder>>();
        
        int count = transform.childCount;
        for(int i = 0; i < count; i++)
        {
            YSortOrder ySortOrder = transform.GetChild(i).GetComponent<YSortOrder>();
            if (ySortOrder.IsUnityNull()) continue;

            Transform tr = transform.GetChild(i);
            if (!dictionary.ContainsKey(tr.position.y))
            {
                List<YSortOrder> ySortOrders = new List<YSortOrder>();
                ySortOrders.Add(ySortOrder);
                dictionary.Add(tr.position.y, ySortOrders);
            }
            else
            {
                if(dictionary.TryGetValue(tr.position.y, out var ySortOrders))
                {
                    ySortOrders.Add(ySortOrder);
                }
            }
        }

        foreach(var kvp in dictionary)
        {
            float yKey = kvp.Key;
            List<YSortOrder> ySortOrders = kvp.Value;

            ySortOrders.Sort((a, b) => a.transform.position.x.CompareTo(b.transform.position.x));

            int bias = 0;
            foreach (var ySort in ySortOrders)
            {
                Undo.RecordObject(ySort, "Assign Sorting Bias");

                ySort.sortingOrderBias = bias%40;
                EditorUtility.SetDirty(ySort);
                bias++;

                Debug.Log($"Y = {yKey}, Set bias = {ySort.sortingOrderBias} to {ySort.gameObject.name}");
            }
        }
    }

    [ContextMenu("FindSame")]
    public void FindSame()
    {
        Dictionary<float, List<YSortOrder>> dictionary = new Dictionary<float, List<YSortOrder>>();

        int count = transform.childCount;
        for (int i = 0; i < count; i++)
        {
            YSortOrder ySortOrder = transform.GetChild(i).GetComponent<YSortOrder>();
            if (ySortOrder.IsUnityNull()) continue;

            Transform tr = transform.GetChild(i);
            if (!dictionary.ContainsKey(tr.position.y))
            {
                List<YSortOrder> ySortOrders = new List<YSortOrder>();
                ySortOrders.Add(ySortOrder);
                dictionary.Add(tr.position.y, ySortOrders);
            }
            else
            {
                if (dictionary.TryGetValue(tr.position.y, out var ySortOrders))
                {
                    ySortOrders.Add(ySortOrder);
                }
            }
        }

        foreach (var kvp in dictionary)
        {
            float yKey = kvp.Key;
            List<YSortOrder> ySortOrders = kvp.Value;

            int bias = 0;

            for(int i = 0; i < ySortOrders.Count; i++)
            {
                for(int j = i + 1; j < ySortOrders.Count; j++)
                {
                    if (ySortOrders[i].sortingOrderBias == ySortOrders[j].sortingOrderBias)
                    {
                        Debug.Log($"ySortOrders[i].name: {ySortOrders[i].name} == ySortOrders[j].name: {ySortOrders[j].name}");
                    }    
                }
            }
        }
    }
}
