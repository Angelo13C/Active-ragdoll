using Unity.Entities;
using UnityEngine;

public class UseCardIfNearPreyAuthoring : MonoBehaviour
{
	[SerializeField] private GameObject _prefabOfCardToUse;
	[SerializeField] [Min(0)] private float _maxDistanceToUseCard = 2f;
	
	class Baker : Baker<UseCardIfNearPreyAuthoring>
	{
		public override void Bake(UseCardIfNearPreyAuthoring authoring)
		{
			var useCardIfNearPrey = new UseCardIfNearPrey
			{
				CardToUse = GetEntity(authoring._prefabOfCardToUse, TransformUsageFlags.None),
				MaxDistanceToUseCardSqr = authoring._maxDistanceToUseCard * authoring._maxDistanceToUseCard
			};
			AddComponent(GetEntity(TransformUsageFlags.None), useCardIfNearPrey);
		}
	}
}