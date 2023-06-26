using Unity.Entities;
using Unity.Physics.Authoring;
using UnityEngine;

public class TornadoAuthoring : MonoBehaviour
{
	[SerializeField] private float _pullAcceleration = 50f;
	[SerializeField] private float _pullRadius = 15f;
	[SerializeField] private float _pullHeight = 100f;
	[SerializeField] private PhysicsCategoryTags _pullablePhysicsTags;
	[SerializeField] private float _riseAcceleration = 1f;
	
	class Baker : Baker<TornadoAuthoring>
	{
		public override void Bake(TornadoAuthoring authoring)
		{
			var entity = GetEntity(authoring, TransformUsageFlags.None);
			
			var tornado = new Tornado
			{
				PullAcceleration = authoring._pullAcceleration,
				PullRadius = authoring._pullRadius,
				PullHeight = authoring._pullHeight,
				PullablePhysicsTags = authoring._pullablePhysicsTags,
				RiseAcceleration = authoring._riseAcceleration
			};
			AddComponent(entity, tornado);
			AddBuffer<SuckedInTornado>(entity);
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.gray;
		Gizmos.DrawWireSphere(transform.position, _pullRadius);
	}
}