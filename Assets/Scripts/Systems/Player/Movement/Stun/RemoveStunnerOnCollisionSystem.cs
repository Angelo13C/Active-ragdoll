using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;

[UpdateInGroup(typeof(PhysicsSystemGroup))]
[UpdateAfter(typeof(StrengthMultiplierSystem))]
[BurstCompile]
public partial struct RemoveStunnerOnCollisionSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var entityCommandBuffer = SystemAPI.GetSingleton<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged);
        state.Dependency = new Job 
        {
            RemoveStunnerOnCollisionLookup = SystemAPI.GetComponentLookup<RemoveStunnerOnCollision>(true),
            EntityCommandBuffer = entityCommandBuffer
        }.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);
    }

    [BurstCompile]
    private struct Job : ICollisionEventsJob
    {
        [ReadOnly] public ComponentLookup<RemoveStunnerOnCollision> RemoveStunnerOnCollisionLookup;
        public EntityCommandBuffer EntityCommandBuffer;

        public void Execute(CollisionEvent collisionEvent)
        {
            if(RemoveStunnerOnCollisionLookup.HasComponent(collisionEvent.EntityA))
                EntityCommandBuffer.RemoveComponent<Stunner>(collisionEvent.EntityA);
            if(RemoveStunnerOnCollisionLookup.HasComponent(collisionEvent.EntityB))
                EntityCommandBuffer.RemoveComponent<Stunner>(collisionEvent.EntityB);
        }
    }
}