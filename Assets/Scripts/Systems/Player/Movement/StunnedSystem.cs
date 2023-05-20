using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

[BurstCompile]
public partial struct StunnedSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var deltaTime = SystemAPI.Time.DeltaTime;
        var rootLookup = SystemAPI.GetComponentLookup<StrengthMultiplier.Root>(true);
        var balancersControllerLookup = SystemAPI.GetComponentLookup<BalancersController>(false);
        foreach (var (stunned, physicsVelocity, entity) in SystemAPI.Query<RefRW<Stunned>, PhysicsVelocity>().WithEntityAccess())
        {
            stunned.ValueRW.Duration -= deltaTime;
            var finished = stunned.ValueRO.Duration <= 0 || stunned.ValueRO.MaxSpeedToRemoveStunSqr >= math.lengthsq(physicsVelocity.Linear);
            if(finished)
                SystemAPI.SetComponentEnabled<Stunned>(entity, false);
            if (rootLookup.TryGetComponent(entity, out var root))
            {
                if (balancersControllerLookup.HasComponent(root.RootEntity))
                {
                    balancersControllerLookup.SetComponentEnabled(root.RootEntity, !stunned.ValueRO.CompleteStun || finished);
                }
            }
        }
    }
}