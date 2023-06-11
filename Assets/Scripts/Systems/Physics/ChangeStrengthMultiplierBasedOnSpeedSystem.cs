using Unity.Burst;
using Unity.Entities;
using Unity.Physics;

[BurstCompile]
public partial struct ChangeStrengthMultiplierBasedOnVelocitySystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (changeStrengthMultiplierBasedOnSpeed, strengthMultiplier, physicsVelocity) in SystemAPI
                     .Query<ChangeStrengthMultiplierBasedOnSpeed, RefRW<StrengthMultiplier>, PhysicsVelocity>())
        {
            var percentage = changeStrengthMultiplierBasedOnSpeed.GetPercentage(physicsVelocity.Linear);
            strengthMultiplier.ValueRW.ForceMultiplierOnCollision =
                changeStrengthMultiplierBasedOnSpeed.StrengthMultiplierAtMaxSpeed * percentage;
        }
    }
}