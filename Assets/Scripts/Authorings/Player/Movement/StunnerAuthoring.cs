using Unity.Entities;
using UnityEngine;

public class StunnerAuthoring : MonoBehaviour
{
	[SerializeField] [Min(0)] private float _newLinearDamping = 1f;
	[SerializeField] private Stunned _stun;

	class Baker : Baker<StunnerAuthoring>
	{
		public override void Bake(StunnerAuthoring authoring)
		{
			var stunner = new Stunner
			{
				NewLinearDamping = authoring._newLinearDamping,
				Stun = authoring._stun
			};
			AddComponent(GetEntity(authoring, TransformUsageFlags.None), stunner);
		}
	}
}