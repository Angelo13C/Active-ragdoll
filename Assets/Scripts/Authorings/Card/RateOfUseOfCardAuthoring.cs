using Unity.Entities;
using UnityEngine;

public class RateOfUseOfCardAuthoring : MonoBehaviour
{
	[SerializeField] [Min(0f)] private float _minDelayBetweenUseOfCards = 1f;

	class Baker : Baker<RateOfUseOfCardAuthoring>
	{
		public override void Bake(RateOfUseOfCardAuthoring authoring)
		{
			var rateOfUseOfCard = new RateOfUseOfCard
			{
				MinDelayBetweenUseOfCards = authoring._minDelayBetweenUseOfCards,
				CurrentUseTimer = 0f
			};
			AddComponent(GetEntity(authoring, TransformUsageFlags.None), rateOfUseOfCard);
		}
	}
}