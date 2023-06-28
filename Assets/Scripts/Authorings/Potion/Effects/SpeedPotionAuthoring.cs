using Unity.Entities;
using UnityEngine;

public class SpeedPotionAuthoring : PotionAuthoring
{
	[SerializeField] private float _speedMultiplier = 1.5f;

	class Baker : Baker<SpeedPotionAuthoring>
	{
		public override void Bake(SpeedPotionAuthoring authoring)
		{
			var entity = GetEntity(authoring, TransformUsageFlags.None);
			
			var potion = authoring.BakePotion(PotionType.Speed);
			AddComponent(entity, potion);

			var speedPotionEffect = new SpeedPotionEffect
			{
				SpeedMultiplier = authoring._speedMultiplier
			};
			AddComponent(entity, speedPotionEffect);
		}
	}
}