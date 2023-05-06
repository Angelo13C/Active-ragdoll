using Unity.Burst;
using Unity.Entities;

[BurstCompile]
public partial struct BalancerLegsControllerSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var balancerLookup = SystemAPI.GetComponentLookup<Balancer>();
        foreach (var (controlledLegs, mover) in SystemAPI.Query<DynamicBuffer<ControlledBalancerLeg>, Mover>())
        {
            var shouldMove = mover.LocalMoveDirection.x != 0 || mover.LocalMoveDirection.y != 0;
            foreach (var controlledLeg in controlledLegs)
            {
                SystemAPI.SetComponentEnabled<BalancerTween>(controlledLeg.Leg, shouldMove);
                if (!shouldMove)
                {
                    var legBalancer = balancerLookup.GetRefRW(controlledLeg.Leg, false);
                    legBalancer.ValueRW.TargetAngle = new PolarCoordinates
                    {
                        Yaw = 0,
                        Pitch = 0
                    };
                }
            }
        }
    }
}
