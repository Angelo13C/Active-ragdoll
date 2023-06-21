using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

[BurstCompile]
public partial struct LightningSpawnSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var entityCommandBuffer = SystemAPI.GetSingleton<EndVariableRateSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged);
        var rng = new Random(1 + (uint) ((SystemAPI.Time.ElapsedTime + SystemAPI.Time.DeltaTime) * 10000));
        foreach (var (lightning, lineRendererPoints, lightningTransform, entity) in SystemAPI.Query<LightningBolt, DynamicBuffer<LineRenderer.Point>, LocalToWorld>().WithAll<LightningTrigger>().WithEntityAccess())
        {
            SystemAPI.SetComponentEnabled<LightningTrigger>(entity, false);
            
            var points = lightning.Generate(rng, Allocator.Temp);
            LineRenderer.Point.SetPoints(lineRendererPoints, points);

            if (SystemAPI.HasComponent<LightningSubBoltsGenerator>(entity))
            {
                var lightningSubBoltsGenerator = SystemAPI.GetComponent<LightningSubBoltsGenerator>(entity);
                var subBolts = lightningSubBoltsGenerator.Generate(points, rng, Allocator.Temp);
                var subBoltsEntities = new NativeArray<Entity>(subBolts.SubBoltsCount, Allocator.Temp,
                    NativeArrayOptions.UninitializedMemory);
                entityCommandBuffer.Instantiate(lightningSubBoltsGenerator.SubBoltPrefab, subBoltsEntities);
                for (var i = 0; i < subBoltsEntities.Length; i++)
                {
                    var (subBoltPoints, transform) = subBolts.GetNthSubBolt(i);
                    var subBoltPointsBuffer = entityCommandBuffer.SetBuffer<LineRenderer.Point>(subBoltsEntities[i]);
                    LineRenderer.Point.SetPoints(subBoltPointsBuffer, subBoltPoints);
                    
                    entityCommandBuffer.AddComponent(subBoltsEntities[i], new Parent
                    {
                        Value = entity
                    });
                    entityCommandBuffer.SetComponent(subBoltsEntities[i], LocalTransform.FromMatrix(transform));
                    entityCommandBuffer.SetComponent(subBoltsEntities[i],  new LocalToWorld
                    {
                        Value = math.mul(lightningTransform.Value, transform)
                    });
                }
            }
        }
    }
}