using Unity.Burst;
using Unity.Entities;

[BurstCompile]
[UpdateInGroup(typeof(CheckUseCardInHandSystemGroup))]
public partial struct CheckRateOfUseOfCardSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var deltaTime = SystemAPI.Time.DeltaTime;
        foreach (var (cardInHandUser, rate) in SystemAPI.Query<RefRW<CardInHandUser>, RefRW<RateOfUseOfCard>>())
        {
            rate.ValueRW.CurrentUseTimer -= deltaTime;
            if (cardInHandUser.ValueRO.TryingToUse != CardInHandUser.CardAction.None)
            {
                if (!rate.ValueRO.CanUse)
                    cardInHandUser.ValueRW.TryingToUse = CardInHandUser.CardAction.None;
            }
        }
    }
}

[BurstCompile]
[UpdateInGroup(typeof(UseCardInHandSystemGroup))]
public partial struct ResetRateOfUseOfCardSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (cardInHandUser, rate) in SystemAPI.Query<CardInHandUser, RefRW<RateOfUseOfCard>>())
        {
            if (cardInHandUser.TryingToUse != CardInHandUser.CardAction.None)
            {
                rate.ValueRW.ResetTimer();
            }
        }
    }
}