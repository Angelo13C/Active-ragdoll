using Unity.Entities;
using UnityEngine;

public class RagdollBodyReferenceAuthoring : MonoBehaviour
{
	[SerializeField] private GameObject _body;

	class Baker : Baker<RagdollBodyReferenceAuthoring>
	{
		public override void Bake(RagdollBodyReferenceAuthoring authoring)
		{
			var ragdollBodyReference = new RagdollBodyReference
			{
				Body = GetEntity(authoring._body, TransformUsageFlags.None)
			};
			AddComponent(GetEntity(authoring, TransformUsageFlags.None), ragdollBodyReference);
		}
	}
}