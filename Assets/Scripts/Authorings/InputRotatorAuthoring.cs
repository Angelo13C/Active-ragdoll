using Unity.Entities;
using UnityEngine;

public class InputRotatorAuthoring : MonoBehaviour
{
	class Baker : Baker<InputRotatorAuthoring>
	{
		public override void Bake(InputRotatorAuthoring authoring)
		{
			AddComponent<InputRotator>(GetEntity(TransformUsageFlags.None));
		}
	}
}