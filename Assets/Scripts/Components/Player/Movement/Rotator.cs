using Unity.Entities;

public struct Rotator : IComponentData
{
    public float Speed;

    public float DeltaYRotation;

    public Entity BalancersControllerEntity;
}