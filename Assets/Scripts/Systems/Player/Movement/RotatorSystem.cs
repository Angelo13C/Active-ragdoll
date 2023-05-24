using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

[BurstCompile]
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public partial struct RotatorSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var deltaTime = SystemAPI.Time.DeltaTime;
        var balancersControllerLookup = SystemAPI.GetComponentLookup<BalancersController>();
        foreach (var (rotator, velocity) in SystemAPI.Query<RefRW<Rotator>, RefRW<PhysicsVelocity>>().WithNone<Stunned>())
        {
            var yRotation = rotator.ValueRO.DeltaYRotation * rotator.ValueRO.Speed * deltaTime;
            velocity.ValueRW.Angular += new float3(0, yRotation, 0);
            rotator.ValueRW.DeltaYRotation -= yRotation;

            var balancersController = balancersControllerLookup.GetRefRWOptional(rotator.ValueRO.BalancersControllerEntity, false);
            if (balancersController.IsValid)
            {
                balancersController.ValueRW.YRotationOffset += yRotation;
                balancersController.ValueRW.YRotationOffset %= 360f;
            }
        }
    }
}