using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Authoring;
using UnityEngine;

public class ChangeLinearDampingOnPunchAuthoring : MonoBehaviour
{
	[SerializeField] private float _newLinearDrag = 0.15f;
	[SerializeField] private CustomPhysicsBodyTags _punchCustomTag;

	class Baker : Baker<ChangeLinearDampingOnPunchAuthoring>
	{
		public override void Bake(ChangeLinearDampingOnPunchAuthoring authoring)
		{
			var changeLinearDampingOnPunch = new ChangeLinearDampingOnPunch
			{
				NewLinearDrag = authoring._newLinearDrag,
				PunchCustomTag = authoring._punchCustomTag.Value
			};
			AddComponent(GetEntity(TransformUsageFlags.None), changeLinearDampingOnPunch);
		}
	}
}