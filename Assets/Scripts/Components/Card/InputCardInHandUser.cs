using Unity.Entities;
using UnityEngine;

public struct InputCardInHandUser : IComponentData
{
    public KeyCode LeftCardKeyCode;
    public KeyCode RightCardKeyCode;

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

    public Entity Get(InputCardInHandUser.CardAction cardAction)
    {
        if (cardAction == InputCardInHandUser.CardAction.None)
            return Entity.Null;
        return cardAction == InputCardInHandUser.CardAction.Left ? LeftCard : RightCard;
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
    public static int ToCardIndex(this InputCardInHandUser.CardAction cardAction)
    {
        if (cardAction == InputCardInHandUser.CardAction.None)
            return -1;
        return cardAction == InputCardInHandUser.CardAction.Left ? 0 : 1;
    }
}