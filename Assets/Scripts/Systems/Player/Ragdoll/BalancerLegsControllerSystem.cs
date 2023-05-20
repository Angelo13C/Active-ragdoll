using Unity.Burst;
using Unity.Entities;

[BurstCompile]
public partial struct BalancerLegsControllerSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var balancerLookup = SystemAPI.GetComponentLookup<Balancer>();
        var balancerTweenLookup = SystemAPI.GetComponentLookup<BalancerTween>();
        var stunnedLookup = SystemAPI.GetComponentLookup<Stunned>();
        foreach (var (controlledLegs, mover, entity) in SystemAPI.Query<DynamicBuffer<ControlledBalancerLeg>, Mover>().WithEntityAccess())
        {
            var shouldMove = mover.LocalMoveDirection.x != 0 || mover.LocalMoveDirection.y != 0;
            if (shouldMove)
                shouldMove = !stunnedLookup.IsComponentEnabled(entity);
            for(var i = 0; i < controlledLegs.Length; i++)
            {
                var controlledLeg = controlledLegs[i];
                balancerTweenLookup.SetComponentEnabled(controlledLeg.Leg, shouldMove);
                if (!shouldMove)
                {
                    var legBalancer = balancerLookup.GetRefRW(controlledLeg.Leg, false);
                    legBalancer.ValueRW.TargetAngle = new PolarCoordinates
                    {
                        Yaw = 0,
                        Pitch = 0
                    };
                }
                else
                {
                    var moveDirection = mover.LocalMoveDirection.y > 0 ? ControlledBalancerLeg.MoveDirection.Forward : ControlledBalancerLeg.MoveDirection.Backward;
                    if (controlledLeg.CurrentMoveDirection != moveDirection)
                    {
                        controlledLegs.ElementAt(i).CurrentMoveDirection = moveDirection;
                        
                        var legBalancerTween = balancerTweenLookup.GetRefRW(controlledLeg.Leg, false);
                        legBalancerTween.ValueRW.FromTargetAngle = -legBalancerTween.ValueRO.FromTargetAngle;
                        legBalancerTween.ValueRW.ToTargetAngle = -legBalancerTween.ValueRO.ToTargetAngle;
                    }
                }
            }
        }
    }
}
