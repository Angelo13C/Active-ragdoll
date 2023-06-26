using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics.Authoring;

public struct Tornado : IComponentData
{
    public float PullAcceleration;
    public float PullHeight;
    public float PullRadius;
    public PhysicsCategoryTags PullablePhysicsTags;
    public float RiseAcceleration;
}

public struct SuckedInTornado : IBufferElementData
{
    public Entity Entity;
    public float3 Position;
}