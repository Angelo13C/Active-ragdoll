using Unity.Burst;
using Unity.Entities;

[BurstCompile]
public partial struct StunHitByExplosionSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (stunned, entity) in SystemAPI.Query<RefRW<Stunned>>().WithAll<HitByExplosion>().WithOptions(EntityQueryOptions.IgnoreComponentEnabledState).WithEntityAccess())
        {
            SystemAPI.SetComponentEnabled<Stunned>(entity, true);
            stunned.ValueRW.Duration = 20f;
            stunned.ValueRW.CompleteStun = true;
        }
    }
}