using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public struct LightningBolt : IComponentData
{
    public float Height;
    public float MaxSpreadWidth;
    public float2 SegmentLengthRange;
    public float SpeedInPointsPerSecond;

    public static int AverageCapacity(float height, float2 segmentLengthRange)
    {
        return (int)(1.5f * height / math.lerp(segmentLengthRange.x, segmentLengthRange.y, 0.5f));
    }

    public NativeArray<float3> Generate(Random rng, Allocator allocator)
    {
        var averageSegmentsCount = AverageCapacity(Height, SegmentLengthRange);
        var points = new NativeList<float3>(averageSegmentsCount, allocator);
        points.Add(float3.zero);

        while (-points[^1].y < Height)
        {
            var x = rng.NextFloat(-MaxSpreadWidth, MaxSpreadWidth);
            var y = points[^1].y - rng.NextFloat(SegmentLengthRange.x, SegmentLengthRange.y);
            var z = 0f;
            points.Add(new float3(x, y, z));
        }
        
        return points;
    }
}

public struct LightningBoltDisappear : IComponentData
{
    public float CurrentTime;
    public float StartTimeToDisappear;
    public float DisappearDuration;
    public float MaxWidth;

    private float CurrentPercentage => 1f - math.clamp(math.unlerp(StartTimeToDisappear, StartTimeToDisappear + DisappearDuration,
                                                CurrentTime), 0, 1);
    public float CurrentWidth => MaxWidth * CurrentPercentage;
}