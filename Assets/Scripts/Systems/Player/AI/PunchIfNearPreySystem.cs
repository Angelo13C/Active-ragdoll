using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct PunchIfNearPreySystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var localToWorldLookup = SystemAPI.GetComponentLookup<LocalToWorld>();
        var playedAnimationsLookup = SystemAPI.GetBufferLookup<PlayedAnimation>();
        var animationsCollection = SystemAPI.GetSingleton<PlayerRagdollAnimationsCollection>();
        foreach (var (predator, punchIfNearPrey, predatorTransform) in SystemAPI.Query<Predator, PunchIfNearPrey, LocalToWorld>())
        {
            if (state.EntityManager.Exists(predator.Prey))
            {
                if (localToWorldLookup.TryGetComponent(predator.Prey, out var preyTransform))
                {
                    var distanceSqr = math.distancesq(predatorTransform.Position, preyTransform.Position);
                    if (distanceSqr < punchIfNearPrey.MaxDistanceToPunchSqr)
                    {
                        if (playedAnimationsLookup.TryGetBuffer(punchIfNearPrey.AnimationPlayer, out var playedAnimations))
                        {
                            animationsCollection.PlayAnimation(animationsCollection.Punch, playedAnimations);
                        }
                    }
                }
            }
        }
    }
}