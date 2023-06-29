using Unity.Entities;
using UnityEngine;

public struct InputCardInHandUser : IComponentData
{
    public KeyCode LeftCardKeyCode;
    public KeyCode RightCardKeyCode;
}

public struct CardInHandUser : IComponentData
{
    public CardAction TryingToUse;
    public enum CardAction
    {
        None,
        Left,
        Right
    }
}

public struct CurrentlyUsingCards : IComponentData
{
    public Entity LeftCard;
    public Entity RightCard;

    public Entity Get(CardInHandUser.CardAction cardAction)
    {
        if (cardAction == CardInHandUser.CardAction.None)
            return Entity.Null;
        return cardAction == CardInHandUser.CardAction.Left ? LeftCard : RightCard;
    }
}

public struct CardUsedBy : IComponentData
{
    public Entity UsedBy;

    [System.Flags]
    public enum Use
    {
        None = 0,
        First = (1 << 0),
        Middle = (1 << 1),
        Last = (1 << 2)
    }
    public Use UseType;

    public CardUsedBy(Entity usedBy, Use useType)
    {
        UsedBy = usedBy;
        UseType = useType;
    }
}

public static class CardActionExtensions
{
    public static int ToCardIndex(this CardInHandUser.CardAction cardAction)
    {
        if (cardAction == CardInHandUser.CardAction.None)
            return -1;
        return cardAction == CardInHandUser.CardAction.Left ? 0 : 1;
    }
}