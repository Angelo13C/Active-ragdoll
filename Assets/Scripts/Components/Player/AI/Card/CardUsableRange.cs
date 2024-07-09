using Unity.Entities;
using Unity.Mathematics;

public struct CardUsableRange : IComponentData
{
    public float2 UsableRangeSqr;
    public float PercentageAddedWhenInsideRangeForASecond;
    public float PercentageRemovedWhenOutsideRangeForASecond;
}