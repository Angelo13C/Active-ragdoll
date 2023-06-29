using Unity.Entities;

public struct RateOfUseOfCard : IComponentData
{
    public float MinDelayBetweenUseOfCards;
    // This value starts from `MinDelayBetweenUseOfCards` when you use a card and decreases to 0,
    // when you will be able to use another card
    public float CurrentUseTimer;

    public void ResetTimer() => CurrentUseTimer = MinDelayBetweenUseOfCards;
    public bool CanUse => CurrentUseTimer <= 0f;
}