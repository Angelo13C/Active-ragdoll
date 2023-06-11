using Unity.Entities;

public struct Stunnable : IComponentData
{
    public float MaxSpeedToRemoveStunSqr;
}

[System.Serializable]
public struct Stunned : IComponentData, IEnableableComponent
{
    //The entity will recover from stun after this extra time passed after MaxSpeedToRemoveStun is reached
    public float ExtraTimeToWaitAfterMaxSpeedRemove;

    //If true the balancers controller will also be disabled
    public bool Faint;
}