using Unity.Entities;
using Unity.Mathematics;

public struct Balancer : IComponentData, IEnableableComponent
{
    public float3 TargetAngle;
    public float Force;
    //I might remove this parent rotation?
    public float3 ParentRotation;
    public float OffsetTargetYAngle;
}
