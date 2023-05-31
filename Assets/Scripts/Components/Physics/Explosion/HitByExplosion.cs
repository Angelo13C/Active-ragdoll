using Unity.Entities;

public struct HitByExplosion : IComponentData
{
    public float RemoveTimer;
    public float DampingBeforeExplosion;

    public bool CanBeRemoved => RemoveTimer <= 0f;

    public HitByExplosion(float dampingBeforeExplosion)
    {
        RemoveTimer = 0.5f;
        DampingBeforeExplosion = dampingBeforeExplosion;
    }
}