using Unity.Entities;
using UnityEngine;

public class StunnedAuthoring : MonoBehaviour
{
	[SerializeField] private float _maxSpeedToRemoveStun = 0.5f;
	
	class Baker : Baker<StunnedAuthoring>
	{
		public override void Bake(StunnedAuthoring authoring)
		{
			var entity = GetEntity(authoring, TransformUsageFlags.None);
			var stunned = new Stunned
			{
				MaxSpeedToRemoveStunSqr = authoring._maxSpeedToRemoveStun * authoring._maxSpeedToRemoveStun,
				Duration = 0
			};
			AddComponent(entity, stunned);
			SetComponentEnabled<Stunned>(entity, false);
		}
	}
}