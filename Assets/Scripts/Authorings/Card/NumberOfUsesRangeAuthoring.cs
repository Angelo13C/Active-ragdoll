using Unity.Entities;
using UnityEngine;

public class NumberOfUsesRangeAuthoring : MonoBehaviour
{
	[SerializeField] private Vector2Int _numberOfUsesRange = new Vector2Int(1, 1);

	class Baker : Baker<NumberOfUsesRangeAuthoring>
	{
		public override void Bake(NumberOfUsesRangeAuthoring authoring)
		{
			var numberOfUsesRange = new NumberOfUsesRange
			{
				From = (short) authoring._numberOfUsesRange.x,
				To = (short) authoring._numberOfUsesRange.y,
			};
			AddComponent(GetEntity(authoring, TransformUsageFlags.None), numberOfUsesRange);
		}
	}
}