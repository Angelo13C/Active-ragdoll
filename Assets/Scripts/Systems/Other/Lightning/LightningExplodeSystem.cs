using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

[BurstCompile]
public partial struct LightningExplodeSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var entityCommandBuffer = SystemAPI.GetSingleton<EndVariableRateSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged);
        foreach (var (lineRenderer, lightningPoints, explosive, lightningTransform, entity) in SystemAPI.Query<LineRenderer, DynamicBuffer<LineRenderer.Point>, Explosive, LocalToWorld>().WithAll<LightningBolt>().WithEntityAccess())
        {
            if (lineRenderer.DisplayPercentage >= 0.95f)
            {
                var explodePosition = lightningTransform.Position + lightningPoints[^1].Position;
                explosive.Explode(entityCommandBuffer, explodePosition);
                entityCommandBuffer.RemoveComponent<Explosive>(entity);
            }
        }
    }
}