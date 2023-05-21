using Unity.Entities;
using UnityEngine;

public class PunchAuthoring : MonoBehaviour
{
	[SerializeField] private float _strengthMultiplier = 800f;
	
	class Baker : Baker<PunchAuthoring>
	{
		public override void Bake(PunchAuthoring authoring)
		{
			var punch = new Punch
			{
				StrengthMultiplier = authoring._strengthMultiplier
			};
			AddComponent(GetEntity(authoring, TransformUsageFlags.None), punch);
		}
	}
}