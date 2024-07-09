using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct UseCardIfNearPreySystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);
        
        var localToWorldLookup = SystemAPI.GetComponentLookup<LocalToWorld>();
        foreach (var (predator, useCardIfNearPrey, predatorTransform, entity) in SystemAPI.Query<Predator, RefRW<UseCardIfNearPrey>, LocalToWorld>().WithEntityAccess())
        {
            if (useCardIfNearPrey.ValueRO.CardToUse != Entity.Null && state.EntityManager.Exists(predator.Prey))
            {
                if (localToWorldLookup.TryGetComponent(predator.Prey, out var preyTransform))
                {
                    var distanceSqr = math.distancesq(predatorTransform.Position, preyTransform.Position);
                    if (distanceSqr < useCardIfNearPrey.ValueRO.MaxDistanceToUseCardSqr)
                    {
                        useCardIfNearPrey.ValueRW.CardToUse = Entity.Null;
                        
                        var usedCard = entityCommandBuffer.Instantiate(useCardIfNearPrey.ValueRO.CardToUse);
                        entityCommandBuffer.AddComponent(usedCard, new CardUsedBy(entity, CardUsedBy.Use.First));
                    }
                }
            }
        }
        
        entityCommandBuffer.Playback(state.EntityManager);
    }
}