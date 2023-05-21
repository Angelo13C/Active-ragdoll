using System;
using TMPro;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;
using Deck = Unity.Entities.DynamicBuffer<CardInDeck>;

public class HandUI : MonoBehaviour
{
    [SerializeField] private Card[] _cards = new Card[2];

    [Serializable]
    private struct Card
    {
        public Image Icon;
        public TextMeshProUGUI LeftUsesText;
    }

    private void Update()
    {
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var handQuery = entityManager.CreateEntityQuery(
            ComponentType.ReadOnly<Hand>(), ComponentType.ReadOnly<CardInDeck>());;
        if(handQuery.TryGetSingleton<Hand>(out var hand) && handQuery.TryGetSingletonBuffer<CardInDeck>(out var deck))
        {
            var cardsInHand = hand.GetCardsInHand(deck);
            for (var i = 0; i < _cards.Length && i < cardsInHand.Length; i++)
            {
                if(_cards[i].Icon != null)
                    _cards[i].Icon.sprite = entityManager.GetComponentData<Icon>(cardsInHand[i].CardPrefab).Sprite;

                if (_cards[i].LeftUsesText != null && cardsInHand[i].LeftUseCount != CardInDeck.INVALID_LEFT_USE_COUNT)
                    _cards[i].LeftUsesText.text = "x" + cardsInHand[i].LeftUseCount;
            }
        }
    }
}