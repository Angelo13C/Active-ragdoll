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
        foreach (var (inputCardInHandUser, rate) in SystemAPI.Query<RefRW<InputCardInHandUser>, RefRW<RateOfUseOfCard>>())
        {
            rate.ValueRW.CurrentUseTimer -= deltaTime;
            if (inputCardInHandUser.ValueRO.TryingToUse != InputCardInHandUser.CardAction.None)
            {
                if (!rate.ValueRO.CanUse)
                    inputCardInHandUser.ValueRW.TryingToUse = InputCardInHandUser.CardAction.None;
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
        foreach (var (inputCardInHandUser, rate) in SystemAPI.Query<InputCardInHandUser, RefRW<RateOfUseOfCard>>())
        {
            if (inputCardInHandUser.TryingToUse != InputCardInHandUser.CardAction.None)
            {
                rate.ValueRW.ResetTimer();
            }
        }
    }
}