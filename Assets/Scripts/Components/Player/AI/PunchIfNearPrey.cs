using Unity.Entities;

public struct PunchIfNearPrey : IComponentData
{
    public float MaxDistanceToPunchSqr;
    public Entity AnimationPlayer;
}