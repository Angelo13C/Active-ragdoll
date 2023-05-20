using Unity.Entities;

public struct ResetLinearDampingWhenStill : IComponentData, IEnableableComponent
{
    public float DefaultLinearDamping;
    public float MaxSpeedToConsiderStillSqr;
}