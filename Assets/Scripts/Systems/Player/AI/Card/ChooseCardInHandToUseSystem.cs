using Unity.Burst;
using Unity.Entities;
using UnityEngine;
using Random = Unity.Mathematics.Random;

[BurstCompile]
[UpdateInGroup(typeof(TriggerUseCardInHandSystemGroup))]
public partial struct ChooseCardInHandToUseSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var rng = new Random(1 + (uint) ((SystemAPI.Time.ElapsedTime + SystemAPI.Time.DeltaTime) * 10000));
        foreach (var (chooseCardInHandToUse, cardInHandUser) in SystemAPI
                     .Query<ChooseCardInHandToUse, RefRW<CardInHandUser>>())
        {
            cardInHandUser.ValueRW.TryingToUse = chooseCardInHandToUse.Use(rng);
        }
    }
}

[BurstCompile]
[UpdateInGroup(typeof(UseCardInHandSystemGroup))]
public partial struct ResetChooseCardInHandToUseSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (cardInHandUser, chooseCardInHandToUse) in SystemAPI.Query<CardInHandUser, RefRW<ChooseCardInHandToUse>>())
        {
            if (cardInHandUser.TryingToUse != CardInHandUser.CardAction.None)
            {
                chooseCardInHandToUse.ValueRW.SetPercentage(cardInHandUser.TryingToUse.ToCardIndex(), 0);
            }
        }
    }
}