using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

[BurstCompile]
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(PhysicsSystemGroup))]
//[UpdateInGroup(typeof(VariableRateSimulationSystemGroup))]
public partial struct TornadoSystem : ISystem
{/*
    public void OnCreate(ref SystemState state)
    {
        var rateManager = new RateUtils.VariableRateManager(200, false);
    }*/
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld;
        foreach (var (tornado, transform, suckedInTornado) in SystemAPI.Query<Tornado, LocalTransform, DynamicBuffer<SuckedInTornado>>())
        {
            var bottom = transform.Position - new float3(0f, tornado.PullRadius, 0f);
            var top = transform.Position + new float3(0f, tornado.PullHeight, 0f);
            var hits = new NativeList<DistanceHit>(10, Allocator.Temp);
            var filter = new CollisionFilter
            {
                BelongsTo = ~0u,
                CollidesWith = tornado.PullablePhysicsTags.Value,
                GroupIndex = CollisionFilter.Default.GroupIndex
            };
            if (physicsWorld.OverlapCapsule(bottom, top, tornado.PullRadius, ref hits, filter,
                    QueryInteraction.IgnoreTriggers))
            {
                suckedInTornado.ResizeUninitialized(hits.Length);
                for (var i = 0; i < hits.Length; i++)
                {
                    suckedInTornado.ElementAt(i) = new SuckedInTornado
                    {
                        Entity = hits[i].Entity,
                        Position = hits[i].Position
                    };
                }
            }
            else
                suckedInTornado.ResizeUninitialized(0);
        }
    }
}