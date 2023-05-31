using Unity.Entities;
using Unity.Physics.Authoring;
using UnityEngine;

public class ExplosiveAuthoring : MonoBehaviour
{
    [SerializeField] private float _force;
    [SerializeField] private float _radius;
    [SerializeField] private float _upwardsModifier = 0.5f;
	[SerializeField] private PhysicsCategoryTags _hittablePhysicsTags;
	[SerializeField] private bool _destroyOnExplosion = true;

	[Space] [SerializeField] private GameObject _explosionPrefab;

	class Baker : Baker<ExplosiveAuthoring>
	{
		public override void Bake(ExplosiveAuthoring authoring)
		{
			var explosive = new Explosive {
				Config = new ExplosionConfig {
					Force = authoring._force,
					Radius = authoring._radius,
					UpwardsModifier = authoring._upwardsModifier,
					HittablePhysicsTags = authoring._hittablePhysicsTags
				},
				DestroyOnExplosion = authoring._destroyOnExplosion,
				ExplosionPrefab = GetEntity(authoring._explosionPrefab, TransformUsageFlags.None),
			};
			
			AddComponent(GetEntity(authoring, TransformUsageFlags.None), explosive);
		}
	}

#if UNITY_EDITOR
	private void OnDrawGizmos() {
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, _radius);
	}
#endif
}