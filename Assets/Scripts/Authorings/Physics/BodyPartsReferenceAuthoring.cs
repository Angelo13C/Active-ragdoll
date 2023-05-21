using Unity.Entities;
using UnityEngine;

public class BodyPartsReferenceAuthoring : MonoBehaviour
{
	[SerializeField] private GameObject _body;
	[SerializeField] private GameObject _rightLowerArm;

	class Baker : Baker<BodyPartsReferenceAuthoring>
	{
		public override void Bake(BodyPartsReferenceAuthoring authoring)
		{
			var ragdollBodyReference = new BodyPartsReference
			{
				Body = GetEntity(authoring._body, TransformUsageFlags.None),
				RightLowerArm = GetEntity(authoring._rightLowerArm, TransformUsageFlags.None)
			};
			AddComponent(GetEntity(authoring, TransformUsageFlags.None), ragdollBodyReference);
		}
	}
}