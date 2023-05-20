using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

[UpdateInGroup(typeof(BeforePhysicsSystemGroup))]
[BurstCompile]
public partial struct MoverSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var(mover, velocity, transform) in SystemAPI.Query<Mover, RefRW<PhysicsVelocity>, LocalToWorld>().WithNone<Stunned>())
        {
            var moveDirection = new float3(mover.LocalMoveDirection.x, 0, mover.LocalMoveDirection.y);
            moveDirection = math.mul(transform.Rotation, moveDirection);
            moveDirection.y = 0;

            var maxSpeed = mover.LocalMoveDirection.x != 0 ? mover.LateralForce :
                mover.LocalMoveDirection.y > 0 ? mover.ForwardForce : mover.BackwardForce;
            var moveSpeed = math.normalizesafe(moveDirection) * maxSpeed;

            velocity.ValueRW.Linear += moveSpeed;
        }
    }
}
