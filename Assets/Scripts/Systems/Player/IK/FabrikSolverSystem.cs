using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct FabrikSolverSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (ikSolver, bonesAndEntities) in SystemAPI.Query<IKSolver, DynamicBuffer<IKBoneAndEntity>>())
        {
            if (bonesAndEntities.Length > 0)
            {
                var bones = new NativeArray<FabrikBone>(bonesAndEntities.Length, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
                for (var i = 0; i < bones.Length; i++)
                    bones[i] = bonesAndEntities[i].Bone;

                ikSolver.Solve(bones);
                for (var i = 0; i < bones.Length; i++)
                    bonesAndEntities.ElementAt(i).Bone = bones[i];

                bones.Dispose();
            }
        }
    }
}
