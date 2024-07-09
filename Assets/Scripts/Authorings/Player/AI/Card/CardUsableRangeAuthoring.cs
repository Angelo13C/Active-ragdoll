using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class CardUsableRangeAuthoring : MonoBehaviour
{
	[SerializeField] private float2 _usableRange = new float2(0, 1f);
	[SerializeField] [Min(0)] private float _percentageAddedWhenInsideRangeForASecond = 50f;
	[SerializeField] [Min(0)] private float _percentageRemovedWhenOutsideRangeForASecond = 500f;

	private void OnValidate()
	{
		_usableRange.x = math.max(0, _usableRange.x);
		_usableRange.y = math.max(_usableRange.x, _usableRange.y);
	}

	class Baker : Baker<CardUsableRangeAuthoring>
	{
		public override void Bake(CardUsableRangeAuthoring authoring)
		{
			var cardUsableRange = new CardUsableRange
			{
				UsableRangeSqr = new float2(authoring._usableRange.x * authoring._usableRange.x, authoring._usableRange.y * authoring._usableRange.y),
				PercentageAddedWhenInsideRangeForASecond = authoring._percentageAddedWhenInsideRangeForASecond,
				PercentageRemovedWhenOutsideRangeForASecond = authoring._percentageRemovedWhenOutsideRangeForASecond
			};
			AddComponent(GetEntity(authoring, TransformUsageFlags.None), cardUsableRange);
		}
	}
}