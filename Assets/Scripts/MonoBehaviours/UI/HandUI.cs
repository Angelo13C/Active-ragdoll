using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;
using Deck = Unity.Entities.DynamicBuffer<CardInDeck>;

public class HandUI : MonoBehaviour
{
    [SerializeField] private Image[] _cardsIcons = new Image[2];

    private void Update()
    {
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var handQuery = entityManager.CreateEntityQuery(
            ComponentType.ReadOnly<Hand>(), ComponentType.ReadOnly<CardInDeck>());;
        if(handQuery.TryGetSingleton<Hand>(out var hand) && handQuery.TryGetSingletonBuffer<CardInDeck>(out var deck))
        {
            var cardsInHand = hand.GetCardsInHand(deck);
            for (var i = 0; i < _cardsIcons.Length && i < cardsInHand.Length; i++)
            {
                if(_cardsIcons[i] != null)
                    _cardsIcons[i].sprite = entityManager.GetComponentData<Icon>(cardsInHand[i].CardPrefab).Sprite;
            }
        }
    }
}