using Unity.Entities;
using UnityEngine;

public class PunchIfNearPreyAuthoring : MonoBehaviour
{
	[SerializeField] [Min(0)] private float _maxDistanceToPunch = 2f;
	
	class Baker : Baker<PunchIfNearPreyAuthoring>
	{
		public override void Bake(PunchIfNearPreyAuthoring authoring)
		{
			var punchIfNearPrey = new PunchIfNearPrey
			{
				MaxDistanceToPunchSqr = authoring._maxDistanceToPunch * authoring._maxDistanceToPunch,
				AnimationPlayer = GetEntity(authoring.GetComponentInParent<PlayerRagdollAnimationPlayerAuthoring>(), TransformUsageFlags.None)
			};
			AddComponent(GetEntity(TransformUsageFlags.None), punchIfNearPrey);
		}
	}
}