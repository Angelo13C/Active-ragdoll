using Unity.Entities;

public struct Throw : IComponentData, IEnableableComponent
{
    public Entity PrefabToThrow;
    public Entity ThrownEntity;
    
    public float ThrowSpeed;
    public float ThrowCooldown;
    public float RemainingTimeUntilThrow;
}