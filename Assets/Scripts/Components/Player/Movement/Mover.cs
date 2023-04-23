using Unity.Entities;
using Unity.Mathematics;

public struct Mover : IComponentData
{
    public float ForwardForce;
    public float BackwardForce;
    public float LateralForce;
    public float2 LocalMoveDirection;
}
