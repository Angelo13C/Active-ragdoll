using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class FirstPersonCamera : IComponentData
{
    public Transform CameraTransform;

    public float VerticalRotationSensitivity;
    public float CurrentVerticalRotation;
    public float2 VerticalRotationLimits;

    public Entity Body;
    public float3 OffsetFromBody;
}