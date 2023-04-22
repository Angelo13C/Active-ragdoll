using Unity.Burst;
using Unity.Entities;
using UnityEngine;

[BurstCompile]
public partial struct InputRotatorSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var rotator in SystemAPI.Query<RefRW<Rotator>>().WithAll<InputRotator>())
        {
            rotator.ValueRW.DeltaYRotation = Input.GetAxis("Mouse X");
        }
    }
}