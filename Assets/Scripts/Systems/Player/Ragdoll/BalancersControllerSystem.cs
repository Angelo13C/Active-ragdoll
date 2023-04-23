using Unity.Burst;
using Unity.Entities;

[BurstCompile]
public partial struct BalancersControllerSystem : ISystem
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
        var balancerLookup = SystemAPI.GetComponentLookup<Balancer>();
        foreach (var(balancersController, controlledBalancers, entity) in SystemAPI
                 .Query<BalancersController, DynamicBuffer<ControlledBalancer>>()
                 .WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)
                 .WithEntityAccess())
        {
            var isEnabled = SystemAPI.IsComponentEnabled<BalancersController>(entity);
            foreach (var controlledBalancer in controlledBalancers)
            {
                SystemAPI.SetComponentEnabled<Balancer>(controlledBalancer.BalancerEntity, isEnabled);
                if (isEnabled)
                {
                    var balancer = balancerLookup.GetRefRW(controlledBalancer.BalancerEntity, false);
                    balancer.ValueRW.OffsetTargetYAngle = balancersController.YRotationOffset;
                }
            }
        }
    }
}
