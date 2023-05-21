using Unity.Entities;
using Deck = Unity.Entities.DynamicBuffer<CardInDeck>;

public struct CardInDeck : IBufferElementData
{
    public Entity CardPrefab;
    // Number of times the card can still be used
    public short LeftUseCount;

    public const short INVALID_LEFT_USE_COUNT = short.MinValue;
}

public static class DeckExtensions
{
    public static void Shuffle(this Deck deck)
    {
        for (var i = 0; i < deck.Length - 1; i++) {
            var r = UnityEngine.Random.Range(i, deck.Length);
            (deck[i], deck[r]) = (deck[r], deck[i]);
        }
    }
    
    public static void AddCard(this Deck deck, Entity cardPrefab)
    {
        deck.Add(new CardInDeck
        {
            CardPrefab = cardPrefab
        });
    }
    
    public static (Entity, bool) UseCardAndEventuallyPutAtEnd(this Deck deck, int cardIndex)
    {
        if (deck.IsEmpty)
            return (Entity.Null, false);

        var usedCard = deck[cardIndex];
        deck.ElementAt(cardIndex).LeftUseCount--;
        if (deck[cardIndex].LeftUseCount == 0)
        {
            deck.ElementAt(cardIndex).LeftUseCount = CardInDeck.INVALID_LEFT_USE_COUNT;
            PutCardAtEnd(deck, cardIndex);
        }
        return (usedCard.CardPrefab, usedCard.LeftUseCount == 1);
    }
    
    public static void PutCardAtEnd(this Deck deck, int cardIndex)
    {
        if (deck.IsEmpty)
            return;
        
        var cardToPutAtEnd = deck[cardIndex];
        for (var i = 0; i < deck.Length - 1; i++)
            deck[i] = deck[i + 1];
        deck[^1] = cardToPutAtEnd;
    }
}