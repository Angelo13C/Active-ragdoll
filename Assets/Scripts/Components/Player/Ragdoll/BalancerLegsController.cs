using Unity.Entities;

public struct BalancerLegsController : IComponentData
{
}

public struct ControlledBalancerLeg : IBufferElementData
{
    public Entity Leg;
    public MoveDirection CurrentMoveDirection;
    
    public enum MoveDirection
    {
        Forward,
        Backward
    }
}
