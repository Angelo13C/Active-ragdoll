using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;

[UpdateInGroup(typeof(AfterPhysicsSystemGroup))]
[UpdateBefore(typeof(ExplodeSystem))]
[BurstCompile]
public partial struct RemoveHitByExplosionOnCollisionSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var deltaTime = SystemAPI.Time.DeltaTime;
        var existsAtLeastOne = false;
        foreach (var hitByExplosion in SystemAPI.Query<RefRW<HitByExplosion>>())
        {
            hitByExplosion.ValueRW.RemoveTimer -= deltaTime;
            existsAtLeastOne = true;
        }

        if (existsAtLeastOne && SystemAPI.TryGetSingleton<RemoveHitByExplosion>(out var removeHitByExplosion))
        {
            var entityCommandBufferSystem = SystemAPI.GetSingleton<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>();
            var entityCommandBuffer = entityCommandBufferSystem.CreateCommandBuffer(state.WorldUnmanaged);
            state.Dependency = new CollisionEventJob
            {
                PhysicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld,
                HitByExplosionLookup = SystemAPI.GetComponentLookup<HitByExplosion>(true),
                PhysicsDampingLookup = SystemAPI.GetComponentLookup<PhysicsDamping>(false),
                RootLookup = SystemAPI.GetComponentLookup<StrengthMultiplier.Root>(true),
                BodyPartsReferenceLookup = SystemAPI.GetComponentLookup<BodyPartsReference>(true),
                EntityCommandBuffer = entityCommandBuffer,
                RemoveHitByExplosion = removeHitByExplosion
            }.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);
        }
    }
    
    [BurstCompile]
    public partial struct CollisionEventJob : ICollisionEventsJob
    {
        [ReadOnly] public PhysicsWorld PhysicsWorld;
        
        [ReadOnly] public ComponentLookup<HitByExplosion> HitByExplosionLookup;
        public ComponentLookup<PhysicsDamping> PhysicsDampingLookup;
        [ReadOnly] public ComponentLookup<StrengthMultiplier.Root> RootLookup;
        [ReadOnly] public ComponentLookup<BodyPartsReference> BodyPartsReferenceLookup;
        public EntityCommandBuffer EntityCommandBuffer;

        public RemoveHitByExplosion RemoveHitByExplosion;

        private void CheckExplode(Entity entity, int otherBodyIndex)
        {
            if (RemoveHitByExplosion.CanRemove(PhysicsWorld.Bodies[otherBodyIndex]))
            {
                if (RootLookup.TryGetComponent(entity, out var root) && BodyPartsReferenceLookup.TryGetComponent(root.RootEntity, out var bodyParts))
                {
                    if (HitByExplosionLookup.TryGetComponent(bodyParts.Body, out var hitByExplosion) &&
                        hitByExplosion.CanBeRemoved)
                    {
                        if (!float.IsNaN(hitByExplosion.DampingBeforeExplosion))
                            PhysicsDampingLookup.GetRefRW(bodyParts.Body, false).ValueRW.Linear = hitByExplosion.DampingBeforeExplosion;

                        EntityCommandBuffer.RemoveComponent<HitByExplosion>(bodyParts.Body);
                    }
                }
            }
        }
        
        public void Execute(CollisionEvent collisionEvent)
        {
            CheckExplode(collisionEvent.EntityA, collisionEvent.BodyIndexB);
            CheckExplode(collisionEvent.EntityB, collisionEvent.BodyIndexA);
        }
    }
}