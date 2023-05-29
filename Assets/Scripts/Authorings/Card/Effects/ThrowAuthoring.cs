using Unity.Entities;
using UnityEngine;

public class ThrowAuthoring : MonoBehaviour
{
	[SerializeField] private GameObject _prefabToThrow;
	
	[SerializeField] [Min(0f)] private float _throwSpeed = 20f;
	[Tooltip("Timer that when expires actually throws the entity")]
	[SerializeField] [Min(0f)] private float _throwCooldown = 0.5f;

	class Baker : Baker<ThrowAuthoring>
	{
		public override void Bake(ThrowAuthoring authoring)
		{
			var throw_ = new Throw
			{
				PrefabToThrow = GetEntity(authoring._prefabToThrow, TransformUsageFlags.None),
				ThrownEntity = Entity.Null,
				ThrowSpeed = authoring._throwSpeed,
				RemainingTimeUntilThrow = authoring._throwCooldown,
				ThrowCooldown = authoring._throwCooldown
			};
			AddComponent(GetEntity(authoring, TransformUsageFlags.None), throw_);
		}
	}
}