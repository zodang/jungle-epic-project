using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class YSortingGroupOrder : MonoBehaviour
{
    public int sortingOrderBias = 0;

    private SortingGroup _sortingGroup;
    private int _yPos;

    private void Awake()
    {
        TryGetComponent<SortingGroup>(out _sortingGroup);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(!_sortingGroup.IsUnityNull())
        {
            _yPos = (int)(transform.position.y * 100f);
            _sortingGroup.sortingOrder = -_yPos;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_sortingGroup.IsUnityNull()) return;

        int newYPos = (int)(transform.position.y * 100f);
        if (_yPos == newYPos) return;

        _yPos = newYPos;
        _sortingGroup.sortingOrder = -_yPos + sortingOrderBias;
    }
}
