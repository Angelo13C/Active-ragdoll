using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct LightningUpdateSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var deltaTime = SystemAPI.Time.DeltaTime;
        var transformLookup = SystemAPI.GetComponentLookup<LocalTransform>(true);
        var lineRendererLookup = SystemAPI.GetComponentLookup<LineRenderer>(false);
        foreach (var (lightning, lineRenderer, lightningPoints, subBolts) in SystemAPI.Query<LightningBolt, RefRW<LineRenderer>, DynamicBuffer<LineRenderer.Point>, DynamicBuffer<Child>>())
        {
            var deltaPercentage = deltaTime * lightning.SpeedInPointsPerSecond / 100f;
            lineRenderer.ValueRW.DisplayPercentage += deltaPercentage;
            if (lineRenderer.ValueRO.DisplayPercentage > 1)
                lineRenderer.ValueRW.DisplayPercentage = 1;
            
            var lowestVisiblePointIndex = math.ceil(lineRenderer.ValueRO.DisplayPercentage * (lightningPoints.Length - 1));
            var lowestVisiblePointY = lightningPoints[(int) lowestVisiblePointIndex].Position.y;
            for (var i = 0; i < subBolts.Length; i++)
            {
                var subBolt = subBolts[i].Value;
                if (transformLookup.TryGetComponent(subBolt, out var subBoltTransform) &&
                    subBoltTransform.Position.y >= lowestVisiblePointY)
                {
                    var subBoltLineRenderer = lineRendererLookup.GetRefRW(subBolt, false);
                    subBoltLineRenderer.ValueRW.DisplayPercentage += deltaPercentage;
                    if (subBoltLineRenderer.ValueRO.DisplayPercentage > 1)
                        subBoltLineRenderer.ValueRW.DisplayPercentage = 1;
                }
            }
        }
    }
}