using Unity.Burst;
using Unity.Entities;

[UpdateInGroup(typeof(UseCardInHandSystemGroup))]
[UpdateBefore(typeof(CardInHandUserSystem))]
[BurstCompile]
public partial struct ResetCardUsedInThisFrameSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var cardUsedBy in SystemAPI.Query<RefRW<CardUsedBy>>())
        {
            cardUsedBy.ValueRW.UseType = CardUsedBy.Use.None;
        }
    }
}