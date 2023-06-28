using Unity.Entities;

public enum PotionType
{
    Speed
}

public struct Potion : IComponentData
{
    public PotionType PotionType;
    public float Duration;
    public float ApplyAfterThisSeconds;
    public const float APPLY_AFTER_THIS_SECONDS = 1f;
}

public struct OriginalPrefab : IComponentData
{
    public Entity Prefab;
}