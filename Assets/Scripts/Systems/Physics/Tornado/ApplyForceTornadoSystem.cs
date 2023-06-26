using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

[BurstCompile]
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(PhysicsSystemGroup))]
public partial struct ApplyForceTornadoSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var deltaTime = SystemAPI.Time.DeltaTime;
        var physicsVelocityLookup = SystemAPI.GetComponentLookup<PhysicsVelocity>(false);
        foreach (var (tornado, transform, suckedInTornado) in SystemAPI.Query<Tornado, LocalTransform, DynamicBuffer<SuckedInTornado>>())
        {
            foreach (var sucked in suckedInTornado)
            {
                var hitVelocity = physicsVelocityLookup.GetRefRWOptional(sucked.Entity, false);
                if (hitVelocity.IsValid)
                {
                    var forceDirection = transform.Position - sucked.Position;
                    var distanceSq = math.lengthsq(forceDirection);
                    var angle = distanceSq > 8 * 8 ? 30f : 90f;
                    forceDirection = math.mul(quaternion.RotateY(math.radians(angle)), forceDirection);
                    forceDirection.y = 0;
                    hitVelocity.ValueRW.Linear += forceDirection * tornado.PullAcceleration * deltaTime;

                    var rise = new float3(0f, tornado.RiseAcceleration, 0f);
                    hitVelocity.ValueRW.Linear += rise * deltaTime;
                }
            }
        }
    }
}