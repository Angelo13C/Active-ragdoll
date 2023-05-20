using Unity.Entities;
using Unity.Physics.Authoring;
using UnityEngine;

[RequireComponent(typeof(PhysicsBodyAuthoring))]
public class ResetLinearDampingWhenStillAuthoring : MonoBehaviour
{
	[SerializeField] private float _maxSpeedToConsiderStill = 0.1f;
	
	class Baker : Baker<ResetLinearDampingWhenStillAuthoring>
	{
		public override void Bake(ResetLinearDampingWhenStillAuthoring authoring)
		{
			var body = authoring.GetComponent<PhysicsBodyAuthoring>();
			var resetLinearDampingWhenStill = new ResetLinearDampingWhenStill
			{
				DefaultLinearDamping = body.LinearDamping,
				MaxSpeedToConsiderStillSqr = authoring._maxSpeedToConsiderStill * authoring._maxSpeedToConsiderStill
			};
			AddComponent(GetEntity(authoring, TransformUsageFlags.None), resetLinearDampingWhenStill);
		}
	}
}