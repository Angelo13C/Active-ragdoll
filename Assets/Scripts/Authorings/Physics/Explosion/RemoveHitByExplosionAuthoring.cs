using Unity.Entities;
using Unity.Physics.Authoring;
using UnityEngine;

public class RemoveHitByExplosionAuthoring : MonoBehaviour
{
	[SerializeField] private CustomPhysicsBodyTags _removeWhenCollidesWithTags;

	class Baker : Baker<RemoveHitByExplosionAuthoring>
	{
		public override void Bake(RemoveHitByExplosionAuthoring authoring)
		{
			var removeHitByExplosion = new RemoveHitByExplosion
			{
				RemoveWhenCollidesWithTags = authoring._removeWhenCollidesWithTags
			};
			AddComponent(GetEntity(authoring, TransformUsageFlags.None), removeHitByExplosion);
		}
	}
}