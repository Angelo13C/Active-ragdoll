using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

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
                    if (strengthMultiplierLookup.HasComponent(bodyPartsReference.RightLowerArm))
                    {
                        strengthMultiplierLookup.SetComponentEnabled(bodyPartsReference.RightLowerArm, true);
                        var strengthMultiplier = strengthMultiplierLookup.GetRefRW(bodyPartsReference.RightLowerArm, false);
                        strengthMultiplier.ValueRW.ForceMultiplierOnCollision *= punch.StrengthMultiplier;
                    }
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
                        entityCommandBuffer.DestroyEntity(entity);

                        if (strengthMultiplierLookup.HasComponent(bodyPartsReference.RightLowerArm))
                        {
                            var strengthMultiplier = strengthMultiplierLookup.GetRefRW(bodyPartsReference.RightLowerArm, false);
                            strengthMultiplier.ValueRW.ForceMultiplierOnCollision /= punch.StrengthMultiplier;
                            strengthMultiplierLookup.SetComponentEnabled(bodyPartsReference.RightLowerArm, false);
                        }
                            
                    }
                }
            }
        }
        entityCommandBuffer.Playback(state.EntityManager);
    }
}