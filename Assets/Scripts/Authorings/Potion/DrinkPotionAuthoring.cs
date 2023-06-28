using Unity.Entities;
using UnityEngine;

public class DrinkPotionAuthoring : MonoBehaviour
{
	[SerializeField] private GameObject _potionPrefab;
	
	class Baker : Baker<DrinkPotionAuthoring>
	{
		public override void Bake(DrinkPotionAuthoring authoring)
		{
			var drinkPotion = new DrinkPotion
			{
				PotionPrefab = GetEntity(authoring._potionPrefab, TransformUsageFlags.None)
			};
			AddComponent(GetEntity(authoring, TransformUsageFlags.None), drinkPotion);
		}
	}
}