using System;
using UnityEngine;

public abstract class Block : MonoBehaviour
{
    public abstract Type RequiredFeatureType { get; }
    public abstract void Activate(object feature);
    public abstract void Deactivate(object feature);
}
