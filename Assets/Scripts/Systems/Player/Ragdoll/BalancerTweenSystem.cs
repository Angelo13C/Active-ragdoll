using Unity.Burst;
using Unity.Entities;

[BurstCompile]
public partial struct BalancerTweenSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var deltaTime = SystemAPI.Time.DeltaTime;
        foreach (var(balancerTween, balancer) in SystemAPI.Query<RefRW<BalancerTween>, RefRW<Balancer>>())
        {
            balancerTween.ValueRW.Update(deltaTime);
            balancer.ValueRW.TargetAngle = balancerTween.ValueRO.Sample();
        }
    }
}
