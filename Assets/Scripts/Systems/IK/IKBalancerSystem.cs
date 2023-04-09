using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

[BurstCompile]
[UpdateAfter(typeof(FabrikSolverSystem))]
public partial struct IKBalancerSystem : ISystem
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
        var balancerLookup = SystemAPI.GetComponentLookup<Balancer>();
        foreach (var ikBones in SystemAPI.Query<DynamicBuffer<IKBoneAndEntity>>().WithAll<IKBalancer>())
        {
            foreach (var ikBone in ikBones)
            {
                var balancer = balancerLookup.GetRefRWOptional(ikBone.Entity, false);

                // The - for the TargetAngle.x maybe should be directly embedded in the YawAndPitch function I think??
                if (balancer.IsValid)
                {
                    var yawAndPitch = math.degrees(ikBone.Bone.YawAndPitchInRadians());
                    balancer.ValueRW.TargetAngle = new float3(-yawAndPitch.x, 0, yawAndPitch.y);
                }
            }
        }
    }
}
