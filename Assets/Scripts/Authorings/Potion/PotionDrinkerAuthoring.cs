using Unity.Entities;
using UnityEngine;

public class PotionDrinkerAuthoring : MonoBehaviour
{
	class Baker : Baker<PotionDrinkerAuthoring>
	{
		public override void Bake(PotionDrinkerAuthoring authoring)
		{
			AddBuffer<DrinkedPotion>(GetEntity(authoring, TransformUsageFlags.None));
		}
	}
}