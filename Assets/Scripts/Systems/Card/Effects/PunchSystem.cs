using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;

[BurstCompile]
public partial struct PunchSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);
        var bodyPartsReferenceLookup = SystemAPI.GetComponentLookup<BodyPartsReference>();
        var strengthMultiplierLookup = SystemAPI.GetComponentLookup<StrengthMultiplier>();
        var playedAnimationsLookup = SystemAPI.GetBufferLookup<PlayedAnimation>();
        var animationsCollection = SystemAPI.GetSingleton<PlayerRagdollAnimationsCollection>();
        foreach (var (punch, cardUsedBy, entity) in SystemAPI.Query<Punch, CardUsedBy>().WithEntityAccess())
        {
            if (cardUsedBy.UseType != CardUsedBy.Use.None)
            {
                if (playedAnimationsLookup.TryGetBuffer(cardUsedBy.UsedBy, out var playedAnimations))
                {
                    animationsCollection.PlayAnimation(animationsCollection.Punch, playedAnimations);
                }

                if (bodyPartsReferenceLookup.TryGetComponent(cardUsedBy.UsedBy, out var bodyPartsReference))
                {
                    if(!SystemAPI.HasComponent<PhysicsCustomTags>(bodyPartsReference.RightLowerArm))
                        entityCommandBuffer.AddComponent(bodyPartsReference.RightLowerArm, new PhysicsCustomTags { Value = Punch.PUNCH_CUSTOM_PHYSICS_BODY_TAGS });
                    else
                    {
                        var bodyCustomTags = SystemAPI.GetComponentRW<PhysicsCustomTags>(bodyPartsReference.RightLowerArm, false);
                        bodyCustomTags.ValueRW.Value |= Punch.PUNCH_CUSTOM_PHYSICS_BODY_TAGS;
                    }

                    var strengthMultiplier = strengthMultiplierLookup.GetRefRWOptional(bodyPartsReference.RightLowerArm, false);
                    if(strengthMultiplier.IsValid)
                        strengthMultiplier.ValueRW.ForceMultiplierOnCollision *= punch.StrengthMultiplier;
                }

                SystemAPI.SetComponentEnabled<Punch>(entity, false);
            }
        }

        foreach (var (cardUsedBy, punch, entity) in SystemAPI.Query<CardUsedBy, Punch>().WithOptions(EntityQueryOptions.IgnoreComponentEnabledState).WithEntityAccess())
        {
            if (playedAnimationsLookup.TryGetBuffer(cardUsedBy.UsedBy, out var playedAnimations))
            {
                if (!animationsCollection.IsAnimationPlaying(animationsCollection.Punch, playedAnimations))
                {
                    if (bodyPartsReferenceLookup.TryGetComponent(cardUsedBy.UsedBy, out var bodyPartsReference))
                    {
                        var bodyCustomTags = SystemAPI.GetComponentRW<PhysicsCustomTags>(bodyPartsReference.RightLowerArm, false);
                        bodyCustomTags.ValueRW.Value &= byte.MaxValue ^ Punch.PUNCH_CUSTOM_PHYSICS_BODY_TAGS;
                        
                        entityCommandBuffer.DestroyEntity(entity);
                        
                        var strengthMultiplier = strengthMultiplierLookup.GetRefRWOptional(bodyPartsReference.RightLowerArm, false);
                        if(strengthMultiplier.IsValid)
                            strengthMultiplier.ValueRW.ForceMultiplierOnCollision /= punch.StrengthMultiplier;
                    }
                }
            }
        }
        entityCommandBuffer.Playback(state.EntityManager);
    }
}