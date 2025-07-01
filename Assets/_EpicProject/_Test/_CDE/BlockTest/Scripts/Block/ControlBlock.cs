using Define;
using System;
using Unity.VisualScripting;
using UnityEngine;

public class ControlBlock : TriggerControlBase<IControllable>
{
    public override BlockType Type => BlockType.PlayerControl;
    public override Type RequiredFeatureType => typeof(IControllable);

    protected override void EnableFeature(IControllable feature)
    {
        CameraFraming cameraFraming = FindAnyObjectByType<CameraFraming>();
        if (!cameraFraming.IsUnityNull() && feature is MonoBehaviour mono)
        {
            cameraFraming.AddTarget(mono.transform);
        }
        
        feature.EnableControl();
    }
    protected override void DisableFeature(IControllable feature)
    {
        CameraFraming cameraFraming = FindAnyObjectByType<CameraFraming>();
        if (!cameraFraming.IsUnityNull() && feature is MonoBehaviour mono)
        {
            if (!mono.CompareTag("Player"))
            {
                cameraFraming.RemoveTarget(mono.transform);
            }
        }
        
        feature.DisableControl();
    }
}
