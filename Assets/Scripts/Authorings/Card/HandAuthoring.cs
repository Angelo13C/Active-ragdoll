using Unity.Entities;
using UnityEngine;

public class HandAuthoring : MonoBehaviour
{
	[SerializeField] private int _maxCardsCount = 2;

	class Baker : Baker<HandAuthoring>
	{
		public override void Bake(HandAuthoring authoring)
		{
			var entity = GetEntity(authoring, TransformUsageFlags.None);
			
			var hand = new Hand
			{
				MaxCardsCount = authoring._maxCardsCount
			};
			AddComponent(entity, hand);
			
			AddComponent<CurrentlyUsingCards>(entity);
		}
	}
}