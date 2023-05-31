using Unity.Entities;
using Unity.Mathematics;

public struct ExplodeOnCollision : IComponentData
{
    public float3 CollisionNormal;
    public float MaxAngleCosine;

    public bool ShouldExplode(float3 collisionNormal)
    {
        return math.dot(collisionNormal, CollisionNormal) >= MaxAngleCosine;
    }
}