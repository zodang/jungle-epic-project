using System;
using System.Collections.Generic;
using Define;
using UnityEngine;

[Serializable]
public class InspectorData
{
    public string Name;
    public Sprite Icon;
    public List<EngineBlock> BlockList = new List<EngineBlock>();
}
