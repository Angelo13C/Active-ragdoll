using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Physics.Systems;

[UpdateInGroup(typeof(PhysicsSystemGroup))]
[UpdateAfter(typeof(PhysicsSimulationGroup))]
[BurstCompile]
public partial struct StrengthMultiplierSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var deltaTime = SystemAPI.Time.DeltaTime;
        foreach (var timers in SystemAPI.Query<DynamicBuffer<StrengthMultiplier.Timer>>())
        {
            for (var i = 0; i < timers.Length; i++)
            {
                timers.ElementAt(i).RemainingTime -= deltaTime;
                if(timers[i].HasExpired)
                    timers.RemoveAtSwapBack(i);
            }
        }
        
        var physicsWorld = SystemAPI.GetSingletonRW<PhysicsWorldSingleton>();
        state.Dependency = new ApplyStrengthMultiplierJob 
        {
            PhysicsWorld = physicsWorld.ValueRW.PhysicsWorld,
            StrengthMultiplierLookup = SystemAPI.GetComponentLookup<StrengthMultiplier>(true),
            RootLookup = SystemAPI.GetComponentLookup<StrengthMultiplier.Root>(true),
            StunAndDamping = new StunAndDamping
            {
                DampingLookup = SystemAPI.GetComponentLookup<PhysicsDamping>(false),
                StunnedLookup = SystemAPI.GetComponentLookup<Stunned>(false),
                RagdollBodyReferenceLookup = SystemAPI.GetComponentLookup<BodyPartsReference>(true)
            },
            StunnerLookup = SystemAPI.GetComponentLookup<Stunner>(true),
            TimerLookup = SystemAPI.GetBufferLookup<StrengthMultiplier.Timer>(false)
        }.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);
    }

    [BurstCompile]
    private struct ApplyStrengthMultiplierJob : ICollisionEventsJob
    {
        public PhysicsWorld PhysicsWorld;
        [ReadOnly] public ComponentLookup<StrengthMultiplier> StrengthMultiplierLookup;
        [ReadOnly] public ComponentLookup<StrengthMultiplier.Root> RootLookup;
        [ReadOnly] public ComponentLookup<Stunner> StunnerLookup;
        
        public StunAndDamping StunAndDamping;

        public BufferLookup<StrengthMultiplier.Timer> TimerLookup;
        
        public void Execute(CollisionEvent collisionEvent)
        {
            var strengthA = StrengthMultiplierLookup.GetRefROOptional(collisionEvent.EntityA);
            var strengthB = StrengthMultiplierLookup.GetRefROOptional(collisionEvent.EntityB);

            if ((strengthA.IsValid && StrengthMultiplierLookup.IsComponentEnabled(collisionEvent.EntityA)) || 
                (strengthB.IsValid && StrengthMultiplierLookup.IsComponentEnabled(collisionEvent.EntityB)))
            {
                var rootA = RootLookup.GetRefROOptional(collisionEvent.EntityA);
                var rootB = RootLookup.GetRefROOptional(collisionEvent.EntityB);
                if (rootA.IsValid && rootB.IsValid && rootA.ValueRO == rootB.ValueRO)
                    return;

                var collisionDetails = collisionEvent.CalculateDetails(ref PhysicsWorld);

                void Do(RefRO<StrengthMultiplier> strength, float impulseDirection, Entity entityHitter, Entity hitEntity, RefRO<StrengthMultiplier.Root> hitRoot, int hitBodyIndex, BufferLookup<StrengthMultiplier.Timer> timerLookup, StunAndDamping stunAndDamping, ComponentLookup<Stunner> stunnerLookup, PhysicsWorld physicsWorld)
                {
                    if (strength.IsValid)
                    {
                        if (timerLookup.TryGetBuffer(entityHitter, out var timers))
                        {
                            var timerEntity = hitRoot.IsValid ? hitRoot.ValueRO.RootEntity : hitEntity;
                            if (!timers.Contains(timerEntity))
                            {
                                timers.Add(new StrengthMultiplier.Timer
                                {
                                    HitEntity = timerEntity,
                                    RemainingTime = 1f
                                });
                            
                                var impulse = collisionEvent.Normal * strength.ValueRO.ForceMultiplierOnCollision * impulseDirection;
                                if(stunnerLookup.TryGetComponent(entityHitter, out var stunner))
                                    stunAndDamping.Apply(timerEntity, stunner.NewLinearDamping, stunner.Stun);

                                physicsWorld.ApplyImpulse(hitBodyIndex, impulse, collisionDetails.AverageContactPointPosition);
                            }
                        }
                    }
                }
                
                Do(strengthA, -1, collisionEvent.EntityA, collisionEvent.EntityB, rootB, collisionEvent.BodyIndexB, TimerLookup, StunAndDamping, StunnerLookup, PhysicsWorld);
                Do(strengthB, 1, collisionEvent.EntityB, collisionEvent.EntityA, rootA, collisionEvent.BodyIndexA, TimerLookup, StunAndDamping, StunnerLookup, PhysicsWorld);
            }
        }
    }
}