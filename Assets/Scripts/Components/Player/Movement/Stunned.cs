using Unity.Entities;

public struct Stunned : IComponentData, IEnableableComponent
{
    public float Duration;
    public float MaxSpeedToRemoveStunSqr;

    //If true the balancers controller will also be disabled
    public bool CompleteStun;
}