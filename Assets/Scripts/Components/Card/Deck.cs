using Unity.Entities;
using Deck = Unity.Entities.DynamicBuffer<CardInDeck>;

public struct CardInDeck : IBufferElementData
{
    public Entity CardPrefab;
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