using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(PhysicsSystemGroup))]
[BurstCompile]
public partial struct ExplodeOnCollisionSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var entityCommandBufferSystem = SystemAPI.GetSingleton<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>();
        var entityCommandBuffer = entityCommandBufferSystem.CreateCommandBuffer(state.WorldUnmanaged);
        state.Dependency = new CollisionEventJob
        {
            PhysicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>(),
            ExplodeOnCollisionLookup = SystemAPI.GetComponentLookup<ExplodeOnCollision>(true),
            ExplosiveLookup = SystemAPI.GetComponentLookup<Explosive>(true),
            EntityCommandBuffer = entityCommandBuffer
        }.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);
    }
    
    [BurstCompile]
    public partial struct CollisionEventJob : ICollisionEventsJob
    {
        [ReadOnly] public PhysicsWorldSingleton PhysicsWorldSingleton;
        
        [ReadOnly] public ComponentLookup<ExplodeOnCollision> ExplodeOnCollisionLookup;
        [ReadOnly] public ComponentLookup<Explosive> ExplosiveLookup;
        public EntityCommandBuffer EntityCommandBuffer;

        private void CheckExplode(Entity entity, CollisionEvent collisionEvent, ref float3? hitPosition)
        {
            if (ExplodeOnCollisionLookup.TryGetComponent(entity, out var explodeOnCollision))
            {
                if (ExplosiveLookup.TryGetComponent(entity, out var explosive))
                {
                    var normalMultiplier = entity == collisionEvent.EntityA ? 1 : -1;
                    var rotation = PhysicsWorldSingleton.PhysicsWorld.Bodies[collisionEvent.BodyIndexA].WorldFromBody.rot;
                    if (explodeOnCollision.ShouldExplode(math.mul(rotation, collisionEvent.Normal * normalMultiplier)))
                    {
                        if (!hitPosition.HasValue)
                            hitPosition = collisionEvent.CalculateDetails(ref PhysicsWorldSingleton.PhysicsWorld)
                                .AverageContactPointPosition;

                        explosive.Explode(EntityCommandBuffer, hitPosition.Value);
                        
                        if(explosive.DestroyOnExplosion)
                            EntityCommandBuffer.DestroyEntity(entity);
                    }
                }
            }
        }
        
        public void Execute(CollisionEvent collisionEvent)
        {
            float3? hitPosition = null;
            CheckExplode(collisionEvent.EntityA, collisionEvent, ref hitPosition);
            CheckExplode(collisionEvent.EntityB, collisionEvent, ref hitPosition);
        }
    }
}