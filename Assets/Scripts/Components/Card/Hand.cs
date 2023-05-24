using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Deck = Unity.Entities.DynamicBuffer<CardInDeck>;

public struct Hand : IComponentData
{
    public int MaxCardsCount;

    public NativeArray<CardInDeck> GetCardsInHand(Deck deck)
    {
        return deck.AsNativeArray().GetSubArray(0, math.min(MaxCardsCount, deck.Length));
    }
}