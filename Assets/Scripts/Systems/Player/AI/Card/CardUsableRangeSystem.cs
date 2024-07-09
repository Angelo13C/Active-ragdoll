using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct CardUsableRangeSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var deltaTime = SystemAPI.Time.DeltaTime;
        var predatorLookup = SystemAPI.GetComponentLookup<Predator>();
        var cardUsableRangeLookup = SystemAPI.GetComponentLookup<CardUsableRange>();
        var localToWorldLookup = SystemAPI.GetComponentLookup<LocalToWorld>();
        foreach (var (deck, chooseCardInHandToUse, bodyPartsReference) in SystemAPI
                     .Query<DynamicBuffer<CardInDeck>, RefRW<ChooseCardInHandToUse>, BodyPartsReference>())
        {
            for (var i = 0; i < 2; i++)
            {
                if (cardUsableRangeLookup.TryGetComponent(deck[i].CardPrefab, out var cardUsableRange)
                    && predatorLookup.TryGetComponent(bodyPartsReference.Body, out var predator)
                    && localToWorldLookup.TryGetComponent(bodyPartsReference.Body, out var transform))
                {
                    if (localToWorldLookup.TryGetComponent(predator.Prey, out var preyTransform))
                    {
                        var distanceSqr = math.distancesq(transform.Position, preyTransform.Position);
                        var percentageIncrease = 0f;
                        if (distanceSqr >= cardUsableRange.UsableRangeSqr.x && distanceSqr <= cardUsableRange.UsableRangeSqr.y)
                            percentageIncrease = cardUsableRange.PercentageAddedWhenInsideRangeForASecond;
                        else
                            percentageIncrease = -cardUsableRange.PercentageRemovedWhenOutsideRangeForASecond;
                        chooseCardInHandToUse.ValueRW.IncreasePercentage(i, percentageIncrease * deltaTime);
                    }
                }
            }
        }
    }
}