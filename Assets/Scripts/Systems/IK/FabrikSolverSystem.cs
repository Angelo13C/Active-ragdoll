using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

[BurstCompile]
public partial struct FabrikSolverSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var(ikSolver, bonesAndEntities, transform) in SystemAPI.Query<IKSolver, DynamicBuffer<IKBoneAndEntity>, WorldTransform>())
        {
            var bones = new NativeArray<FabrikBone>(bonesAndEntities.Length, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            for (var i = 0; i < bones.Length; i++)
                bones[i] = bonesAndEntities[i].Bone;

            ikSolver.Solve(bones, transform.Position);
            for (var i = 0; i < bones.Length; i++)
                bonesAndEntities.ElementAt(i).Bone = bones[i];

            bones.Dispose();
        }
    }
}
