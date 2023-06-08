using Unity.Entities;
using UnityEngine;

public class ChangeLinearDampingOnPunchAuthoring : MonoBehaviour
{
	[SerializeField] private float _newLinearDrag = 0.15f;

	class Baker : Baker<ChangeLinearDampingOnPunchAuthoring>
	{
		public override void Bake(ChangeLinearDampingOnPunchAuthoring authoring)
		{
			var changeLinearDampingOnPunch = new ChangeLinearDampingOnPunch
			{
				NewLinearDrag = authoring._newLinearDrag,
			};
			AddComponent(GetEntity(TransformUsageFlags.None), changeLinearDampingOnPunch);
		}
	}
}