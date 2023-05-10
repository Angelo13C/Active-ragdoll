using Unity.Entities;

public struct FollowPrey : IComponentData
{
    public float MinDistanceFromPreySqr;
}