using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct FollowPreySystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var localToWorldLookup = SystemAPI.GetComponentLookup<LocalToWorld>();
        var balancersControllerLookup = SystemAPI.GetComponentLookup<BalancersController>();
        foreach (var (predator, followPrey, predatorTransform, mover, rotator) in SystemAPI.Query<Predator, FollowPrey, LocalToWorld, RefRW<Mover>, RefRW<Rotator>>())
        {
            var forwardMovement = 0f;
            var neededYRotation = 0f;
            
            if(localToWorldLookup.TryGetComponent(predator.Prey, out var preyTransform))
            {
                var preyDirection = preyTransform.Position - predatorTransform.Position;
                if (math.lengthsq(preyDirection) > followPrey.MinDistanceFromPreySqr)
                    forwardMovement = 1;
                
                if (balancersControllerLookup.TryGetComponent(rotator.ValueRO.BalancersControllerEntity, out var balancersController))
                {
                    neededYRotation = math.degrees(math.atan2(preyDirection.z, preyDirection.x)) - 90f;
                    neededYRotation -= balancersController.YRotationOffset;
                    neededYRotation %= 360;
                    if (math.abs(neededYRotation) > 180)
                        neededYRotation += -math.sign(neededYRotation) * 360;
                }
            }

            mover.ValueRW.LocalMoveDirection = new float2(0, forwardMovement);
            rotator.ValueRW.DeltaYRotation = neededYRotation;
        }
    }
}