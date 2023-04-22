using Unity.Entities;
using UnityEngine;

public class RotatorAuthoring : MonoBehaviour
{
	[SerializeField] private float _rotationSpeed = 1;

	class Baker : Baker<RotatorAuthoring>
	{
		public override void Bake(RotatorAuthoring authoring)
		{
			var rotator = new Rotator
			{
				Speed = authoring._rotationSpeed,
				BalancersControllerEntity = GetEntity(authoring.GetComponentInParent<BalancersControllerAuthoring>(), TransformUsageFlags.Dynamic)
			};
			AddComponent(GetEntity(TransformUsageFlags.Dynamic), rotator);
		}
	}
}