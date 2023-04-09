using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public struct IKSolver : IComponentData
{
    public FabrikSolver Solver;
    public bool Local;

    public float3 Target;
    public float3? Pole;

    public void Solve(NativeArray<FabrikBone> bones, float3 solverPosition)
    {
        var offset = Local ? solverPosition : float3.zero;
        Solver.Solve(bones, Target + offset, Pole.HasValue ? Pole.Value + offset : null);
    }
}

public struct IKBoneAndEntity : IBufferElementData
{
    public FabrikBone Bone;
    public Entity Entity;
}

public struct FabrikSolver
{
    private int _maxIterationsCount;
    private float _maxErrorDistanceSqr;

    public FabrikSolver(int maxIterationsCount, float maxErrorDistance)
    {
        _maxIterationsCount = maxIterationsCount;
        _maxErrorDistanceSqr = maxErrorDistance * maxErrorDistance;
    }

    public void Solve(NativeArray<FabrikBone> bones, float3 target, float3? pole)
    {
        if (bones.Length == 0)
            return;

        var start = bones[0].Start;
        var bonesLength = bones.TotalLength();
        if (math.distancesq(start, target) >= bonesLength * bonesLength)
        {
            SolveUnreachable(bones, target);
        }
        else
        {
            if (pole.HasValue)
                Solve(bones, pole.Value, null);

            for (var currentIteration = 0; currentIteration < _maxIterationsCount; currentIteration++)
            {
                if (math.distancesq(bones[bones.Length - 1].End, target) <= _maxErrorDistanceSqr)
                    break;

                IterateReachable(bones, target);
            }
        }
    }

    private void IterateReachable(NativeArray<FabrikBone> bones, float3 target)
    {
        var start = bones[0].Start;

        // Forward pass
        var previousEnd = target;
        for (var i = bones.Length - 1; i >= 0; i--)
        {
            var direction = math.normalize(previousEnd - bones[i].Start);
            previousEnd -= direction * bones[i].Length;
            bones[i] = new FabrikBone
            {
                Start = previousEnd,
                Direction = direction,
                Length = bones[i].Length
            };
        }

        // Backward pass
        var previousStart = start;
        for (var i = 0; i < bones.Length; i++)
        {
            var direction = math.normalize(bones[i].End - previousStart);
            bones[i] = new FabrikBone
            {
                Start = previousStart,
                Direction = direction,
                Length = bones[i].Length
            };
            previousStart = bones[i].End;
        }
    }

    private void SolveUnreachable(NativeArray<FabrikBone> bones, float3 target)
    {
        var startToTargetDirection = math.normalize(target - bones[0].Start);
        bones[0] = new FabrikBone
        {
            Length = bones[0].Length,
            Direction = startToTargetDirection,
            Start = bones[0].Start
        };
        for (var i = 1; i < bones.Length; i++)
        {
            bones[i] = new FabrikBone
            {
                Length = bones[i].Length,
                Direction = startToTargetDirection,
                Start = bones[i - 1].End
            };
        }
    }
}

public struct FabrikBone
{
    public float Length;
    public float3 Direction;
    public float3 Start;

    public float3 End => Start + Direction * Length;

    public float2 YawAndPitchInRadians()
    {
        return new float2(math.atan2(Direction.z, Direction.x), math.asin(-Direction.y));
    }
}

public static class Extensions
{
    public static float TotalLength(this NativeArray<FabrikBone> bones)
    {
        var length = 0f;
        foreach (var bone in bones)
            length += bone.Length;

        return length;
    }
}
