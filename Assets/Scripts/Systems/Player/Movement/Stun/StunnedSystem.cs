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
        foreach (var (stunnable, stunned, physicsVelocity, entity) in SystemAPI.Query<Stunnable, RefRW<Stunned>, PhysicsVelocity>().WithEntityAccess())
        {
            var finished = false;
            if (stunnable.MaxSpeedToRemoveStunSqr >= math.lengthsq(physicsVelocity.Linear))
            {
                stunned.ValueRW.ExtraTimeToWaitAfterMaxSpeedRemove -= deltaTime;
                finished = stunned.ValueRO.ExtraTimeToWaitAfterMaxSpeedRemove <= 0f;
            }
            if(finished)
                SystemAPI.SetComponentEnabled<Stunned>(entity, false);
            
            if (rootLookup.TryGetComponent(entity, out var root))
            {
                if (balancersControllerLookup.HasComponent(root.RootEntity))
                {
                    balancersControllerLookup.SetComponentEnabled(root.RootEntity, !stunned.ValueRO.Faint || finished);
                }
            }
        }
    }
}