using Unity.Burst;
using Unity.Entities;

[BurstCompile]
[UpdateAfter(typeof(FabrikSolverSystem))]
public partial struct IKBalancerSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var balancerLookup = SystemAPI.GetComponentLookup<Balancer>();
        foreach (var ikBones in SystemAPI.Query<DynamicBuffer<IKBoneAndEntity>>().WithAll<IKBalancer>())
        {
            foreach (var ikBone in ikBones)
            {
                var balancer = balancerLookup.GetRefRWOptional(ikBone.Entity, false);
                if (balancer.IsValid)
                    balancer.ValueRW.TargetAngle = ikBone.Bone.ToPolarCoordinates();
            }
        }
    }
}
