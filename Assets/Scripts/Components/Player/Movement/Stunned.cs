using Unity.Entities;

public struct Stunned : IComponentData, IEnableableComponent
{
    public float Duration;
    public float MaxSpeedToRemoveStunSqr;
    //The entity will recover from stun after this extra time passed after MaxSpeedToRemoveStun is reached
    public float ExtraTimeToWaitAfterMaxSpeedRemove;

    //If true the balancers controller will also be disabled
    public bool CompleteStun;
}