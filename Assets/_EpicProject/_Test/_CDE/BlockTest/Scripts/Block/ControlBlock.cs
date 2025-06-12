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
        feature.EnableControl();

        CameraFraming cameraFraming = FindAnyObjectByType<CameraFraming>();
        if (!cameraFraming.IsUnityNull() && feature is MonoBehaviour mono)
        {
            cameraFraming.AddTarget(mono.transform);
        }
    }
    protected override void DisableFeature(IControllable feature)
    {
        feature.DisableControl();

        CameraFraming cameraFraming = FindAnyObjectByType<CameraFraming>();
        if (!cameraFraming.IsUnityNull() && feature is MonoBehaviour mono)
        {
            if (!mono.CompareTag("Player"))
            {
                cameraFraming.RemoveTarget(mono.transform);
            }
        }
    }
}
