using Unity.Burst;
using Unity.Entities;
using UnityEngine;

[UpdateInGroup(typeof(TriggerUseCardInHandSystemGroup))]
[BurstCompile]
public partial struct InputCardInHandUserSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (inputCardInHandUser, cardInHandUser) in SystemAPI.Query<InputCardInHandUser, RefRW<CardInHandUser>>())
        {
            if (Input.GetKeyDown(inputCardInHandUser.LeftCardKeyCode))
                cardInHandUser.ValueRW.TryingToUse = CardInHandUser.CardAction.Left;
            else if (Input.GetKeyDown(inputCardInHandUser.RightCardKeyCode))
                cardInHandUser.ValueRW.TryingToUse = CardInHandUser.CardAction.Right;
            else
                cardInHandUser.ValueRW.TryingToUse = CardInHandUser.CardAction.None;
        }
    }
}