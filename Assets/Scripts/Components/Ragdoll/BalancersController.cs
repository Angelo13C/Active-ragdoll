using Unity.Entities;

public struct BalancersController : IComponentData, IEnableableComponent
{
    public float YRotationOffset;
}

public struct ControlledBalancer : IBufferElementData
{
    public Entity BalancerEntity;
}
