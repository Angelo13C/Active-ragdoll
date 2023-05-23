using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;

[BurstCompile]
public partial struct ConnectJointToCardUserSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (children, cardUsedBy, entity) in SystemAPI.Query<DynamicBuffer<Child>, CardUsedBy>().WithAll<ConnectJointToCardUser>().WithEntityAccess())
        {
            foreach (var child in children)
            {
                if (SystemAPI.HasComponent<PhysicsConstrainedBodyPair>(child.Value))
                {
                    var physicsConstrainedBodyPair = SystemAPI.GetComponentRW<PhysicsConstrainedBodyPair>(child.Value, false);
                    var bodyPartsReferenceToConnect = SystemAPI.GetComponent<BodyPartsReference>(cardUsedBy.UsedBy);
                    physicsConstrainedBodyPair.ValueRW = new PhysicsConstrainedBodyPair(physicsConstrainedBodyPair.ValueRO.EntityA, bodyPartsReferenceToConnect.RightLowerArm, false);
                    SystemAPI.SetComponentEnabled<ConnectJointToCardUser>(entity, false);
                    break;
                }
            }
        }
    }
}