using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;

[UpdateInGroup(typeof(AfterPhysicsSystemGroup))]
[BurstCompile]
public partial struct ResetLinearDampingWhenStillSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (resetLinearDampingWhenStill, physicsVelocity, physicsDamping) in SystemAPI
                     .Query<ResetLinearDampingWhenStill, PhysicsVelocity, RefRW<PhysicsDamping>>().WithNone<HitByExplosion>())
        {
            if (math.lengthsq(physicsVelocity.Linear) <= resetLinearDampingWhenStill.MaxSpeedToConsiderStillSqr)
            {
                physicsDamping.ValueRW.Linear = resetLinearDampingWhenStill.DefaultLinearDamping;
            }
        }
    }
}