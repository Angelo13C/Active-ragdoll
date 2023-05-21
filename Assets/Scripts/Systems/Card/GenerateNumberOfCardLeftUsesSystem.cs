using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

[BurstCompile]
public partial struct GenerateNumberOfCardLeftUsesSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // I don't think this is a great way to generate the RNG's seed
        var rng = new Random(1 + (uint) ((SystemAPI.Time.ElapsedTime + SystemAPI.Time.DeltaTime) * 10000));
        var numberOfUsesRangeLookup = SystemAPI.GetComponentLookup<NumberOfUsesRange>(true);
        foreach (var deck in SystemAPI.Query<DynamicBuffer<CardInDeck>>())
        {
            for (var i = deck.Length - 1; i >= 0; i--)
            {
                if (deck[i].LeftUseCount == CardInDeck.INVALID_LEFT_USE_COUNT)
                {
                    if (numberOfUsesRangeLookup.TryGetComponent(deck[i].CardPrefab, out var range))
                        deck.ElementAt(i).LeftUseCount = (short) rng.NextInt(range.From, range.To + 1);
                }
                else
                    break;
            }
        }
    }
}