using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

[UpdateAfter(typeof(CardInHandUserSystem))]
[UpdateInGroup(typeof(UseCardInHandSystemGroup))]
[BurstCompile]
public partial struct ConnectJointToCardUserSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (children, cardUsedBy, transform, entity) in SystemAPI.Query<DynamicBuffer<LinkedEntityGroup>, CardUsedBy, RefRW<LocalTransform>>().WithAll<ConnectJointToCardUser>().WithEntityAccess())
        {
            foreach (var child in children)
            {
                if (SystemAPI.HasComponent<PhysicsConstrainedBodyPair>(child.Value))
                {
                    var physicsConstrainedBodyPair = SystemAPI.GetComponentRW<PhysicsConstrainedBodyPair>(child.Value, false);
                    var bodyPartsReferenceToConnect = SystemAPI.GetComponent<BodyPartsReference>(cardUsedBy.UsedBy);
                    physicsConstrainedBodyPair.ValueRW = new PhysicsConstrainedBodyPair(physicsConstrainedBodyPair.ValueRO.EntityA, bodyPartsReferenceToConnect.RightLowerArm, false);
                    var joint = SystemAPI.GetComponent<PhysicsJoint>(child.Value);
                    var usedByTransform = SystemAPI.GetComponent<LocalToWorld>(bodyPartsReferenceToConnect.RightLowerArm);
                    transform.ValueRW.Position = math.mul(joint.BodyBFromJoint.AsRigidTransform().rot, joint.BodyBFromJoint.Position) + usedByTransform.Position;
                    SystemAPI.SetComponentEnabled<ConnectJointToCardUser>(entity, false);
                    break;
                }
            }
        }
    }
}