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
        
        ChangeLinearDampingOnPunch.WithLookup? onPunch = null;
        if (SystemAPI.TryGetSingleton<ChangeLinearDampingOnPunch>(out var onPunchSingleton))
        {
            onPunch = new ChangeLinearDampingOnPunch.WithLookup
            {
                ChangeLinearDampingOnPunch = onPunchSingleton,
                DampingLookup = SystemAPI.GetComponentLookup<PhysicsDamping>(false),
                StunnedLookup = SystemAPI.GetComponentLookup<Stunned>(false),
                RagdollBodyReferenceLookup = SystemAPI.GetComponentLookup<BodyPartsReference>(true)
            };
        }

        var physicsWorld = SystemAPI.GetSingletonRW<PhysicsWorldSingleton>();
        state.Dependency = new ApplyStrengthMultiplierJob 
        {
            PhysicsWorld = physicsWorld.ValueRW.PhysicsWorld,
            StrengthMultiplierLookup = SystemAPI.GetComponentLookup<StrengthMultiplier>(true),
            RootLookup = SystemAPI.GetComponentLookup<StrengthMultiplier.Root>(true),
            OnPunch = onPunch,
            TimerLookup = SystemAPI.GetBufferLookup<StrengthMultiplier.Timer>(false)
        }.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);
    }

    [BurstCompile]
    private struct ApplyStrengthMultiplierJob : ICollisionEventsJob
    {
        public PhysicsWorld PhysicsWorld;
        [ReadOnly] public ComponentLookup<StrengthMultiplier> StrengthMultiplierLookup;
        [ReadOnly] public ComponentLookup<StrengthMultiplier.Root> RootLookup;
        public ChangeLinearDampingOnPunch.WithLookup? OnPunch;

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

                void Do(RefRO<StrengthMultiplier> strength, float impulseDirection, Entity entityHitter, Entity hitEntity, RefRO<StrengthMultiplier.Root> hitRoot, int hitBodyIndex, BufferLookup<StrengthMultiplier.Timer> timerLookup, ChangeLinearDampingOnPunch.WithLookup? onPunch, PhysicsWorld physicsWorld)
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
                                if (onPunch.HasValue)
                                    onPunch.Value.ApplyIfRequired(hitRoot);

                                physicsWorld.ApplyImpulse(hitBodyIndex, impulse, collisionDetails.AverageContactPointPosition);
                            }
                        }
                    }
                }
                
                Do(strengthA, -1, collisionEvent.EntityA, collisionEvent.EntityB, rootB, collisionEvent.BodyIndexB, TimerLookup, OnPunch, PhysicsWorld);
                Do(strengthB, 1, collisionEvent.EntityB, collisionEvent.EntityA, rootA, collisionEvent.BodyIndexA, TimerLookup, OnPunch, PhysicsWorld);
            }
        }
    }
}