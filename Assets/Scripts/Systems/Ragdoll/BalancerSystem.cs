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
            var position = physicsWorld.GetPosition(rigidBodyIndex);
            var rotationAngles = balancer.TargetAngle + balancer.ParentRotation;
            rotationAngles.y += balancer.OffsetTargetYAngle;
            var rotation = math.normalizesafe(quaternion.EulerXYZ(math.radians(rotationAngles)));
            var targetTransform = new RigidTransform(rotation, position);
            physicsWorld.CalculateVelocityToTarget(rigidBodyIndex, targetTransform, deltaTime * balancer.Force, out var linearVelocity, out var angularVelocity);
            velocity.ValueRW.Angular = angularVelocity;
        }
    }
}
