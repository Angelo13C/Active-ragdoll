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
        ChangeLinearDampingOnPunch.WithLookup? onPunch = null;
        if (SystemAPI.TryGetSingleton<ChangeLinearDampingOnPunch>(out var onPunchSingleton))
        {
            onPunch = new ChangeLinearDampingOnPunch.WithLookup
            {
                ChangeLinearDampingOnPunch = onPunchSingleton,
                DampingLookup = SystemAPI.GetComponentLookup<PhysicsDamping>(false),
                RootLookup = SystemAPI.GetComponentLookup<StrengthMultiplier.Root>(true),
                StunnedLookup = SystemAPI.GetComponentLookup<Stunned>(false),
                RagdollBodyReferenceLookup = SystemAPI.GetComponentLookup<RagdollBodyReference>(true)
            };
        }
        
        var physicsWorld = SystemAPI.GetSingletonRW<PhysicsWorldSingleton>();
        state.Dependency = new ApplyStrengthMultiplierJob 
        {
            PhysicsWorld = physicsWorld.ValueRW.PhysicsWorld,
            StrengthMultiplierLookup = SystemAPI.GetComponentLookup<StrengthMultiplier>(true),
            RootLookup = SystemAPI.GetComponentLookup<StrengthMultiplier.Root>(true),
            OnPunch = onPunch
        }.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);
    }

    [BurstCompile]
    private struct ApplyStrengthMultiplierJob : ICollisionEventsJob
    {
        public PhysicsWorld PhysicsWorld;
        [ReadOnly] public ComponentLookup<StrengthMultiplier> StrengthMultiplierLookup;
        [ReadOnly] public ComponentLookup<StrengthMultiplier.Root> RootLookup;
        public ChangeLinearDampingOnPunch.WithLookup? OnPunch;

        public void Execute(CollisionEvent collisionEvent)
        {
            var strengthA = StrengthMultiplier.Invalid;
            var strengthB = StrengthMultiplier.Invalid;
            if (StrengthMultiplierLookup.TryGetComponent(collisionEvent.EntityA, out var strengthMultiplierA))
                strengthA = strengthMultiplierA;
            if (StrengthMultiplierLookup.TryGetComponent(collisionEvent.EntityB, out var strengthMultiplierB))
                strengthB = strengthMultiplierB;

            if (strengthA.IsValid || strengthB.IsValid)
            {
                if (RootLookup.TryGetComponent(collisionEvent.EntityA, out var rootA) &&
                    RootLookup.TryGetComponent(collisionEvent.EntityB, out var rootB))
                {
                    if (rootA == rootB)
                        return;
                }

                var collisionDetails = collisionEvent.CalculateDetails(ref PhysicsWorld);

                if (strengthA.IsValid)
                {
                    if (OnPunch.HasValue)
                        OnPunch.Value.ApplyIfRequired(PhysicsWorld.Bodies[collisionEvent.BodyIndexA], collisionEvent.EntityB);
                    PhysicsWorld.ApplyImpulse(collisionEvent.BodyIndexB, -collisionEvent.Normal * (strengthA.ForceMultiplierOnCollision - 1), collisionDetails.AverageContactPointPosition);
                }

                if (strengthB.IsValid)
                {
                    if (OnPunch.HasValue)
                        OnPunch.Value.ApplyIfRequired(PhysicsWorld.Bodies[collisionEvent.BodyIndexB], collisionEvent.EntityA);
                    PhysicsWorld.ApplyImpulse(collisionEvent.BodyIndexA, collisionEvent.Normal * (strengthB.ForceMultiplierOnCollision - 1), collisionDetails.AverageContactPointPosition);
                }
            }
        }
    }
}