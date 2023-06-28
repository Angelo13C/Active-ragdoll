using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[UpdateAfter(typeof(PotionDrinkerSystem))]
public partial struct SpeedPotionSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (drinkedPotions, entity) in SystemAPI.Query<DynamicBuffer<DrinkedPotion>>().WithEntityAccess())
        {
            for (var i = 0; i < drinkedPotions.Length; i++)
            {
                var drinkedPotion = drinkedPotions[i];
                if (drinkedPotion.PotionType == PotionType.Speed)
                {
                    if (drinkedPotion.WasDrunkThisFrame)
                    {
                        var bodyPartsReference = SystemAPI.GetComponent<BodyPartsReference>(entity);
                        var mover = SystemAPI.GetComponentRW<Mover>(bodyPartsReference.Body, false);
                        var speedPotion = SystemAPI.GetComponent<SpeedPotionEffect>(drinkedPotion.Potion);
                        mover.ValueRW.BackwardForce *= speedPotion.SpeedMultiplier;
                        mover.ValueRW.ForwardForce *= speedPotion.SpeedMultiplier;
                        mover.ValueRW.LateralForce *= speedPotion.SpeedMultiplier;
                    }
                    else if (drinkedPotion.EffectEndsThisFrame)
                    {
                        var bodyPartsReference = SystemAPI.GetComponent<BodyPartsReference>(entity);
                        var mover = SystemAPI.GetComponentRW<Mover>(bodyPartsReference.Body, false);
                        var speedPotion = SystemAPI.GetComponent<SpeedPotionEffect>(drinkedPotion.Potion);
                        mover.ValueRW.BackwardForce /= speedPotion.SpeedMultiplier;
                        mover.ValueRW.ForwardForce /= speedPotion.SpeedMultiplier;
                        mover.ValueRW.LateralForce /= speedPotion.SpeedMultiplier;
                    }
                }
            }
        }
    }
}