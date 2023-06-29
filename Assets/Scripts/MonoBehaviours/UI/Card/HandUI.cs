using System;
using System.Collections;
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
        public CardDissolveParticles DissolveParticles;
    }

    private bool[] _currentlyDissolvingCards = new bool[2];

    [Header("Dissolve")]
    [SerializeField] private Material _dissolveEffectMaterial;
    
    [Header("Display new card")]
    [SerializeField] private float _timeToWaitToDisplayCardAfterDissolve = 0.8f;
    [SerializeField] private float _displayNewCardTweenDuration = 0.15f;
    [SerializeField] private LeanTweenType _displayNewCardTweenEase = LeanTweenType.easeInCubic;
    
    private const string START_TIME_REFERENCE = "_StartTime";
    private readonly int START_TIME_ID = Shader.PropertyToID(START_TIME_REFERENCE);

    [Header("Left uses animation")] 
    [SerializeField] private float _leftUsesTextTweenSize = 1.2f;
    [SerializeField] private float _leftUsesTextTweenDuration = 0.3f;
    [SerializeField] private LeanTweenType _leftUsesTextTweenEase = LeanTweenType.easeInCubic;

    // I need to reset LeanTween once somewhere because I have disabled domain reload in Unity and LeanTween wouldn't work without being reset
    private void Awake() => LeanTween.reset();

    private void Update()
    {
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var handQuery = entityManager.CreateEntityQuery(
            ComponentType.ReadOnly<Hand>(), ComponentType.ReadOnly<CardInDeck>(), ComponentType.ReadOnly<InputCardInHandUser>(),
            ComponentType.ReadOnly<CardInHandUser>());;
        if(handQuery.TryGetSingleton<Hand>(out var hand) && handQuery.TryGetSingletonBuffer<CardInDeck>(out var deck) && handQuery.TryGetSingleton<CardInHandUser>(out var cardInHandUser))
        {
            var cardsInHand = hand.GetCardsInHand(deck);
            for (var i = 0; i < _cards.Length && i < cardsInHand.Length; i++)
            {
                if (_cards[i].Icon != null)
                {
                    var newIcon = entityManager.GetComponentData<Icon>(cardsInHand[i].CardPrefab).Sprite;
                    if (newIcon != _cards[i].Icon.sprite && Time.time != 0f)
                    {
                        if (!_currentlyDissolvingCards[i])
                        {
                            _cards[i].Icon.material = _dissolveEffectMaterial;
                            _cards[i].Icon.material.SetFloat(START_TIME_ID, Time.time);
                            _cards[i].DissolveParticles.Dissolve(_cards[i].Icon.sprite);
                            StartCoroutine(ResetMaterial(i, newIcon));
                        }
                    }
                    else
                        _cards[i].Icon.sprite = newIcon;
                }

                if (_cards[i].LeftUsesText != null && cardsInHand[i].LeftUseCount != CardInDeck.INVALID_LEFT_USE_COUNT)
                    _cards[i].LeftUsesText.text = "x" + cardsInHand[i].LeftUseCount;
            }
            
            if (cardInHandUser.TryingToUse != CardInHandUser.CardAction.None)
            {
                var usedCardIndex = cardInHandUser.TryingToUse == CardInHandUser.CardAction.Left ? 0 : 1;
                var leftUsesOfUsedCardText = _cards[usedCardIndex].LeftUsesText;
                LeanTween.scale(leftUsesOfUsedCardText.rectTransform, _leftUsesTextTweenSize * Vector3.one, _leftUsesTextTweenDuration)
                    .setEase(_leftUsesTextTweenEase).setLoopPingPong(1);
            }
        }
    }

    private IEnumerator ResetMaterial(int cardIndex, Sprite sprite)
    {
        _currentlyDissolvingCards[cardIndex] = true;
        
        yield return new WaitForSeconds(_timeToWaitToDisplayCardAfterDissolve);
        _cards[cardIndex].Icon.material = null;
        _cards[cardIndex].Icon.sprite = sprite;
        _cards[cardIndex].Icon.rectTransform.localScale = Vector3.zero;
        LeanTween.scale(_cards[cardIndex].Icon.rectTransform, Vector3.one, _displayNewCardTweenDuration)
            .setEase(_displayNewCardTweenEase);
        
        _currentlyDissolvingCards[cardIndex] = false;
    }
}