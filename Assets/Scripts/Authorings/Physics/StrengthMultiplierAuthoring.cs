using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Authoring;
using UnityEngine;

public class StrengthMultiplierAuthoring : MonoBehaviour
{
	[SerializeField] [Min(0)] private float _forceMultiplierOnCollision = 1000f;

	[SerializeField] private bool _initiallyActive = false;
	
	class Baker : Baker<StrengthMultiplierAuthoring>
	{
		public override void Bake(StrengthMultiplierAuthoring authoring)
		{
			var entity = GetEntity(TransformUsageFlags.None);
			
			GetComponent<PhysicsShapeAuthoring>(authoring).CollisionResponse = CollisionResponsePolicy.CollideRaiseCollisionEvents;
			var strengthMultiplier = new StrengthMultiplier
			{
				ForceMultiplierOnCollision = authoring._forceMultiplierOnCollision
			};
			AddComponent(entity, strengthMultiplier);
			SetComponentEnabled<StrengthMultiplier>(entity, authoring._initiallyActive);
			AddBuffer<StrengthMultiplier.Timer>(entity);
		}
	}
}