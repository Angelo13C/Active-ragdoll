using Unity.Entities;
using UnityEngine;

public class RemoveStunnerOnCollisionAuthoring : MonoBehaviour
{
	class Baker : Baker<RemoveStunnerOnCollisionAuthoring>
	{
		public override void Bake(RemoveStunnerOnCollisionAuthoring authoring)
		{
			AddComponent<RemoveStunnerOnCollision>(GetEntity(authoring, TransformUsageFlags.None));
		}
	}
}