using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

[BurstCompile]
public partial struct LightningDisappearSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var deltaTime = SystemAPI.Time.DeltaTime;
        foreach (var (lightningBoltDisappear, lineRenderer, subBolts) in SystemAPI.Query<RefRW<LightningBoltDisappear>, RefRW<LineRenderer>, DynamicBuffer<Child>>())
        {
            if (lineRenderer.ValueRO.DisplayPercentage >= 1f)
            {
                lightningBoltDisappear.ValueRW.CurrentTime += deltaTime;
                lineRenderer.ValueRW.LineWidth = lightningBoltDisappear.ValueRO.CurrentWidth;

                foreach (var subBolt in subBolts)
                {
                    var subBoltLineRenderer = SystemAPI.GetComponentRW<LineRenderer>(subBolt.Value, false);
                    subBoltLineRenderer.ValueRW.HalfLineWidth = lineRenderer.ValueRO.HalfLineWidth;
                }
            }
        }
    }
}