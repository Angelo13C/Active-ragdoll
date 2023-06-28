using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct DrinkPotionSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var entityCommandBuffer = SystemAPI.GetSingleton<EndVariableRateSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged);
        foreach (var (drinkPotion, cardUsedBy) in SystemAPI.Query<DrinkPotion, CardUsedBy>())
        {
            if (cardUsedBy.UseType != CardUsedBy.Use.None)
            {
                var potion = entityCommandBuffer.Instantiate(drinkPotion.PotionPrefab);
                var cardUsedByBodyPartsReference = SystemAPI.GetComponent<BodyPartsReference>(cardUsedBy.UsedBy);
                entityCommandBuffer.AddComponent(potion, new Parent
                {
                    Value = cardUsedByBodyPartsReference.RightLowerArm
                });
                entityCommandBuffer.AddComponent(potion, new OriginalPrefab
                {
                    Prefab = drinkPotion.PotionPrefab
                });

                if (SystemAPI.HasBuffer<PlayedAnimation>(cardUsedBy.UsedBy))
                {
                    var playedAnimations = SystemAPI.GetBuffer<PlayedAnimation>(cardUsedBy.UsedBy);
                    var animationsCollection = SystemAPI.GetSingleton<PlayerRagdollAnimationsCollection>();
                    animationsCollection.PlayAnimation(animationsCollection.DrinkPotion, playedAnimations);
                }
            }
        }
    }
}