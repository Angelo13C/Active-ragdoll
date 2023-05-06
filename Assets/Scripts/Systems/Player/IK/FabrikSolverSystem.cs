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
        var localToWorldLookup = SystemAPI.GetComponentLookup<LocalToWorld>(true);
        foreach (var (ikSolver, bonesAndEntities) in SystemAPI.Query<IKSolver, DynamicBuffer<IKBoneAndEntity>>())
        {
            if (bonesAndEntities.Length > 0)
            {
                var bones = new NativeArray<FabrikBone>(bonesAndEntities.Length, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
                
                for (var i = 0; i < bones.Length; i++)
                {
                    var bone = bonesAndEntities[i].Bone;
                    if (localToWorldLookup.TryGetComponent(bonesAndEntities[i].Entity, out var boneTransform))
                    {
                        bone.Direction = math.mul(boneTransform.Rotation, bonesAndEntities[i].InitialDirection);
                        bone.Start = boneTransform.Position - bone.Direction * bone.Length / 2;
                    }

                    bones[i] = bone;
                }

                ikSolver.Solve(bones);
                for (var i = 0; i < bones.Length; i++)
                    bonesAndEntities.ElementAt(i).Bone = bones[i];

                bones.Dispose();
            }
        }
    }
}
