using Unity.Entities;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

public struct ChooseCardInHandToUse : IComponentData
{
    public float LeftCardPercentage;
    public float RightCardPercentage;

    public void SetPercentage(int cardIndex, float newPercentage)
    {
        if (cardIndex == 0)
            LeftCardPercentage = math.clamp(newPercentage, 0, 100);
        else
            RightCardPercentage = math.clamp(newPercentage, 0, 100);
    }

    public void IncreasePercentage(int cardIndex, float percentageIncrease)
    {
        var newPercentage = percentageIncrease;
        if (cardIndex == 0)
            newPercentage += LeftCardPercentage;
        else
            newPercentage += RightCardPercentage;
        SetPercentage(cardIndex, newPercentage);
    }

    public CardInHandUser.CardAction Use(Random rng)
    {
        var randomValue = rng.NextFloat(100f);
        var cardAction = CardInHandUser.CardAction.None;
        if (randomValue <= LeftCardPercentage)
            cardAction = CardInHandUser.CardAction.Left;
        if (randomValue <= RightCardPercentage && RightCardPercentage > LeftCardPercentage)
            cardAction = CardInHandUser.CardAction.Right;
        return cardAction;
    }
}