using Unity.Entities;
using UnityEngine;

public class PredatorAuthoring : MonoBehaviour
{
	[SerializeField] private PreyAuthoring _prey;

	class Baker : Baker<PredatorAuthoring>
	{
		public override void Bake(PredatorAuthoring authoring)
		{
			var predator = new Predator
			{
				Prey = GetEntity(authoring._prey, TransformUsageFlags.None)
			};
			AddComponent(GetEntity(TransformUsageFlags.None), predator);
		}
	}
}