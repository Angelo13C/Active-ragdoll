using Unity.Entities;
using UnityEngine;

public class FollowPreyAuthoring : MonoBehaviour
{
	[SerializeField] [Min(0)] private float _minDistanceFromPrey = 1.5f;

	class Baker : Baker<FollowPreyAuthoring>
	{
		public override void Bake(FollowPreyAuthoring authoring)
		{
			var followPrey = new FollowPrey
			{
				MinDistanceFromPreySqr = authoring._minDistanceFromPrey * authoring._minDistanceFromPrey
			};
			AddComponent(GetEntity(authoring, TransformUsageFlags.None), followPrey);
		}
	}
}