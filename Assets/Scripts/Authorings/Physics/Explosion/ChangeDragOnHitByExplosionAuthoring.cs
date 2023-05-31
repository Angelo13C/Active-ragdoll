using Unity.Entities;
using UnityEngine;

public class ChangeDragOnHitByExplosionAuthoring : MonoBehaviour
{
	[SerializeField] private float _newDrag = 5f;

	class Baker : Baker<ChangeDragOnHitByExplosionAuthoring>
	{
		public override void Bake(ChangeDragOnHitByExplosionAuthoring authoring)
		{
			var changeDragOnHitByExplosion = new ChangeDragOnHitByExplosion
			{
				NewDrag = authoring._newDrag
			};
			AddComponent(GetEntity(authoring, TransformUsageFlags.None), changeDragOnHitByExplosion);
		}
	}
}