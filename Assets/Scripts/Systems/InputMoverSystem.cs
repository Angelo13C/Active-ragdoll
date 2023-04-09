using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[BurstCompile]
public partial struct InputMoverSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (inputMover, mover) in SystemAPI.Query<InputMover, RefRW<Mover>>())
        {
            var moveDirection = float2.zero;
            if (Input.GetKey(inputMover.ForwardKey))
                moveDirection.y += 1;
            if (Input.GetKey(inputMover.BackwardKey))
                moveDirection.y -= 1;
            if (Input.GetKey(inputMover.RightKey))
                moveDirection.x += 1;
            if (Input.GetKey(inputMover.LeftKey))
                moveDirection.x -= 1;

            mover.ValueRW.LocalMoveDirection = moveDirection;
        }
    }
}