using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[UpdateBefore(typeof(PotionDrinkerSystem))]
public partial struct RemovePotionEffectSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var deltaTime = SystemAPI.Time.DeltaTime;
        foreach (var drinkedPotions in SystemAPI.Query<DynamicBuffer<DrinkedPotion>>())
        {
            for (var i = drinkedPotions.Length - 1; i >= 0; i--)
            {
                var currentPotion = drinkedPotions[i];
                if(currentPotion.EffectEndsThisFrame)
                    drinkedPotions.RemoveAt(i);
                else
                {
                    currentPotion.TimeLeft -= deltaTime;
                    currentPotion.WasDrunkThisFrame = false;
                    drinkedPotions.ElementAt(i) = currentPotion;
                }
            }
        }
    }
}