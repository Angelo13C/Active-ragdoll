using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public struct LightningTrigger : IComponentData, IEnableableComponent
{
    
}

public struct LightningSubBoltsGenerator : IComponentData
{
    public Entity SubBoltPrefab;
    public int2 SubBoltsCountRange;
    public float2 SubBoltsHeightRange;
    public float2 SubBoltsWidthSpreadRange;
    public float2 SubBoltsSegmentLengthRange;
    
    private NativeArray<LightningBolt> GenerateSubBolts(Random rng, Allocator allocator)
    {
        var subBoltsCount = rng.NextInt(SubBoltsCountRange.x, SubBoltsCountRange.y);
        var subBolts = new NativeArray<LightningBolt>(subBoltsCount, allocator, NativeArrayOptions.UninitializedMemory);
        for (var i = 0; i < subBoltsCount; i++)
        {
            subBolts[i] = new LightningBolt
            {
                Height = rng.NextFloat(SubBoltsHeightRange.x, SubBoltsHeightRange.y),
                MaxSpreadWidth = rng.NextFloat(SubBoltsWidthSpreadRange.x, SubBoltsWidthSpreadRange.y),
                SegmentLengthRange = rng.NextFloat(SubBoltsSegmentLengthRange.x, SubBoltsSegmentLengthRange.y),
            };
        }

        return subBolts;
    }
    
    public Bolts Generate(NativeArray<float3> mainBoltPoints, Random rng, Allocator allocator)
    {
        var subBolts = GenerateSubBolts(rng, allocator);
        var initialCapacity = subBolts.Length * LightningBolt.AverageCapacity(SubBoltsHeightRange.y, SubBoltsSegmentLengthRange);
        var points = new NativeList<float3>(initialCapacity, allocator);

        var subBoltsIndices = new NativeArray<int>(subBolts.Length, allocator, NativeArrayOptions.UninitializedMemory);
        var subBoltsTransforms = new NativeArray<float4x4>(subBolts.Length, allocator, NativeArrayOptions.UninitializedMemory);
        for (var i = 0; i < subBolts.Length; i++)
        {
            subBoltsIndices[i] = points.Length;
            points.AddRange(subBolts[i].Generate(rng, allocator));
            var localPosition = mainBoltPoints[rng.NextInt(mainBoltPoints.Length - 20)];
            var direction = rng.NextBool() ? 1 : -1;
            var localRotation = quaternion.RotateZ(direction * math.radians(rng.NextFloat(30, 70)));
            subBoltsTransforms[i] = float4x4.TRS(localPosition, localRotation, new float3(1, 1, 1));
        }

        return new Bolts(points.AsArray(), subBoltsIndices, subBoltsTransforms);
    }

    public struct Bolts
    {
        private NativeArray<float3> _points;
        private NativeArray<int> _subBoltsIndices;
        private NativeArray<float4x4> _transforms;

        public Bolts(NativeArray<float3> points, NativeArray<int> subBoltsIndices, NativeArray<float4x4> transforms)
        {
            _points = points;
            _subBoltsIndices = subBoltsIndices;
            _transforms = transforms;
        }
        
        public int SubBoltsCount => _subBoltsIndices.Length;
        public (NativeArray<float3>, float4x4) GetNthSubBolt(int n)
        {
            var length = (n == _subBoltsIndices.Length - 1 ? _points.Length : _subBoltsIndices[n + 1]) - _subBoltsIndices[n];
            return (_points.GetSubArray(_subBoltsIndices[n], length), _transforms[n]);
        }
    }
}