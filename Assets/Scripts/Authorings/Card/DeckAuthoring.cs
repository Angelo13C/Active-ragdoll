using Unity.Entities;
using UnityEngine;

public class DeckAuthoring : MonoBehaviour
{
	[SerializeField] private GameObject[] _initialCards = new GameObject[0];

	class Baker : Baker<DeckAuthoring>
	{
		public override void Bake(DeckAuthoring authoring)
		{
			var deck = AddBuffer<CardInDeck>(GetEntity(authoring, TransformUsageFlags.None));
			deck.ResizeUninitialized(authoring._initialCards.Length);
			for (var i = 0; i < deck.Length; i++)
			{
				deck[i] = new CardInDeck
				{
					CardPrefab = GetEntity(authoring._initialCards[i], TransformUsageFlags.None),
					LeftUseCount = CardInDeck.INVALID_LEFT_USE_COUNT
				};
			}
		}
	}
}