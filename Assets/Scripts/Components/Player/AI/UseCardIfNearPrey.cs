using Unity.Entities;

public struct UseCardIfNearPrey : IComponentData
{
    public float MaxDistanceToUseCardSqr;
    public Entity CardToUse;
}