using Unity.Entities;
using UnityEngine;

public class StunnableAuthoring : MonoBehaviour
{
	[SerializeField] private float _maxSpeedToRemoveStun = 1.5f;
	
	class Baker : Baker<StunnableAuthoring>
	{
		public override void Bake(StunnableAuthoring authoring)
		{
			var entity = GetEntity(authoring, TransformUsageFlags.None);
			var stunnable = new Stunnable
			{
				MaxSpeedToRemoveStunSqr = authoring._maxSpeedToRemoveStun * authoring._maxSpeedToRemoveStun
			};
			AddComponent(entity, stunnable);
			
			AddComponent<Stunned>(entity);
			SetComponentEnabled<Stunned>(entity, false);
		}
	}
}