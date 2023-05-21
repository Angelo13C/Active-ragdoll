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
        foreach (var inputCardInHandUser in SystemAPI.Query<RefRW<InputCardInHandUser>>())
        {
            if (Input.GetKeyDown(inputCardInHandUser.ValueRO.LeftCardKeyCode))
                inputCardInHandUser.ValueRW.TryingToUse = InputCardInHandUser.CardAction.Left;
            else if (Input.GetKeyDown(inputCardInHandUser.ValueRO.RightCardKeyCode))
                inputCardInHandUser.ValueRW.TryingToUse = InputCardInHandUser.CardAction.Right;
            else
                inputCardInHandUser.ValueRW.TryingToUse = InputCardInHandUser.CardAction.None;
        }
    }
}