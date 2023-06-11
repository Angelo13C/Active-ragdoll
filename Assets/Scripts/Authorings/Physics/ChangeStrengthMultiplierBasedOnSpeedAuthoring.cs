using Unity.Entities;
using UnityEngine;

public class ChangeStrengthMultiplierBasedOnSpeedAuthoring : MonoBehaviour
{
	[SerializeField] private float _minSpeed = 1f;
	[SerializeField] private float _maxSpeed = 20f;

	class Baker : Baker<ChangeStrengthMultiplierBasedOnSpeedAuthoring>
	{
		public override void Bake(ChangeStrengthMultiplierBasedOnSpeedAuthoring authoring)
		{
			var changeStrengthMultiplierBasedOnSpeed = new ChangeStrengthMultiplierBasedOnSpeed
			{
				MinSpeedSqr = authoring._minSpeed * authoring._minSpeed,
				MaxSpeedSqr = authoring._maxSpeed * authoring._maxSpeed,
				StrengthMultiplierAtMaxSpeed = GetComponent<StrengthMultiplierAuthoring>().ForceMultiplierOnCollision
			};
			AddComponent(GetEntity(authoring, TransformUsageFlags.None), changeStrengthMultiplierBasedOnSpeed);
		}
	}
}