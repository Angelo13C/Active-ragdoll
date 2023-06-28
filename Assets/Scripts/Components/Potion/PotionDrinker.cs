using Unity.Entities;
public struct DrinkedPotion : IBufferElementData
{
    public PotionType PotionType;
    public Entity Potion;
    public float TimeLeft;
    public bool WasDrunkThisFrame;
    public bool EffectEndsThisFrame => TimeLeft <= 0f;
}