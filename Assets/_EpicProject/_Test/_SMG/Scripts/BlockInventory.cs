using System;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;

public class BlockInventory : MonoBehaviour
{
    //private readonly List<DummyBlock> _blocks;
    [SerializeField] private List<DummyBlock> _blocks = new List<DummyBlock>();

    private int _maxCnt = 5;
    public int Count { get { return _blocks.Count; } }

    public bool IsEmpty => Count == 0;
    public bool IsFull => Count >= _maxCnt;


    public Action ChangeBlockInventory;

    //public BlockInventory()
    //{
    //    _blocks = new List<DummyBlock>();
    //}

    //public BlockInventory(int capacity)
    //{
    //    _blocks = new List<DummyBlock>(capacity);
    //}

    //public BlockInventory(IEnumerable<DummyBlock> items)
    //{
    //    _blocks = new List<DummyBlock>(items);
    //}

    public void Initialized()
    {
        Clear();
    }

    public void Clear()
    {
        _blocks.Clear();
        ChangeBlockInventory?.Invoke();
    }

    // 추가
    void AddBlock(DummyBlock block)
    {
        if (IsFull || FindBlock(block)) return;

        _blocks.Add(block);
        ChangeBlockInventory?.Invoke();
    }

    public bool TryAddBlock(DummyBlock block)
    {
        if (IsFull || FindBlock(block)) return false;

        _blocks.Add(block);
        ChangeBlockInventory?.Invoke();
        return true;
    }

    bool FindBlock(DummyBlock block)
    {
        return _blocks.Contains(block);
    }
    
    public DummyBlock TakeBlock(int index)
    {
        if (index < 0 || index >= _blocks.Count) return null;

        DummyBlock block = _blocks[index];
        RemoveBlock(index);
        ChangeBlockInventory?.Invoke();
        return block;
    }

    // 제거
    void RemoveBlock(int index)
    {
        if (index < 0 || index >= _blocks.Count) return;

        _blocks.RemoveAt(index);
        ChangeBlockInventory?.Invoke();
    }

    // 삭제
    void DeleteBlock()
    {
        ChangeBlockInventory?.Invoke();
    }

    // 순서 이동
    public void SwapBlock(int indexA, int indexB)
    {
        if (indexA == indexB) return;
        if (indexA < 0 || indexA >= _blocks.Count) return;
        if (indexB < 0 || indexB >= _blocks.Count) return;

        var temp = _blocks[indexA];
        _blocks[indexA] = _blocks[indexB];
        _blocks[indexB] = temp;

        ChangeBlockInventory?.Invoke();
    }

    //  =   =   =   =   =   ContextMenu
    [Header("ContextMenu")]
    public int testRemoveIndex;
    public int testIndexA;
    public int testIndexB;
    public int testTakeIndex;

    [ContextMenu("RemoveBlock(int testRemoveIndex)")]
    void TestRemove()
    {
        RemoveBlock(testRemoveIndex);
    }

    [ContextMenu("SwapBlock(testIndexA, testIndexB)")]
    void TestSwapBlock()
    {
        SwapBlock(testIndexA, testIndexB);
    }

    [ContextMenu("TakeBlock(testTakeIndex)")]
    void TestTakeBlock()
    {
        Debug.Log(TakeBlock(testTakeIndex).gameObject.name);
    }
}
