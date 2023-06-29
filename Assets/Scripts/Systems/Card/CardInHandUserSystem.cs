using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

[UpdateInGroup(typeof(UseCardInHandSystemGroup))]
[BurstCompile]
public partial struct CardInHandUserSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);
        
        foreach (var (deck, hand, cardInHandUser, currentlyUsingCards, entity) in SystemAPI.Query<DynamicBuffer<CardInDeck>, Hand, CardInHandUser, RefRW<CurrentlyUsingCards>>().WithEntityAccess())
        {
            var cardToUse = cardInHandUser.TryingToUse.ToCardIndex();
            if (cardToUse != -1)
            {
                var (usedCardPrefab, lastUse) = deck.UseCardAndEventuallyPutAtEnd(cardToUse, hand);
                if (usedCardPrefab != Entity.Null)
                {
                    var lastUseType = lastUse ? CardUsedBy.Use.Last : CardUsedBy.Use.None;
                    var currentlyUsedCard = currentlyUsingCards.ValueRO.Get(cardInHandUser.TryingToUse);
                    if (currentlyUsedCard == Entity.Null)
                    {
                        var usedCard = entityCommandBuffer.Instantiate(usedCardPrefab);
                        entityCommandBuffer.AddComponent(usedCard, new CardUsedBy(entity, CardUsedBy.Use.First | lastUseType));

                        if (cardInHandUser.TryingToUse == CardInHandUser.CardAction.Left)
                            currentlyUsingCards.ValueRW.LeftCard = usedCard;
                        else
                            currentlyUsingCards.ValueRW.RightCard = usedCard;
                    }
                    else
                        SystemAPI.SetComponent(currentlyUsedCard, new CardUsedBy(entity, lastUse ? lastUseType : CardUsedBy.Use.Middle));

                    if (lastUse)
                    {
                        if (cardInHandUser.TryingToUse == CardInHandUser.CardAction.Left)
                            currentlyUsingCards.ValueRW.LeftCard = Entity.Null;
                        else
                            currentlyUsingCards.ValueRW.RightCard = Entity.Null;
                    }
                    
                    // This is done to make sure the ECB remaps the new entity of the new used card
                    entityCommandBuffer.SetComponent(entity, currentlyUsingCards.ValueRW);
                }
            }
        }
        
        entityCommandBuffer.Playback(state.EntityManager);
    }
}

public partial class TriggerUseCardInHandSystemGroup : ComponentSystemGroup { }

[UpdateAfter(typeof(TriggerUseCardInHandSystemGroup))]
public partial class CheckUseCardInHandSystemGroup : ComponentSystemGroup { }

[UpdateAfter(typeof(CheckUseCardInHandSystemGroup))]
public partial class UseCardInHandSystemGroup : ComponentSystemGroup { }