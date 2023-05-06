using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Physics.Systems;

[UpdateInGroup(typeof(AfterPhysicsSystemGroup))]
public partial struct BalancerSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {
        var deltaTime = SystemAPI.Time.DeltaTime;
        var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld;
        foreach (var(balancer, velocity, entity) in SystemAPI.Query<Balancer, RefRW<PhysicsVelocity>>().WithEntityAccess())
        {
            var rigidBodyIndex = physicsWorld.GetRigidBodyIndex(entity);
            var targetRotation = balancer.GetTargetRotationQuaternion();
            var targetTransform = new RigidTransform(targetRotation, float3.zero);
            
            physicsWorld.CalculateVelocityToTarget(rigidBodyIndex, targetTransform, deltaTime * balancer.Force, out var linearVelocity, out var angularVelocity);
            velocity.ValueRW.Angular = angularVelocity;
        }
    }
}
