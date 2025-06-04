using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InspectorData
{
    public string Name;
    public Sprite Icon;
    public List<Block> BlockList = new List<Block>();
}
