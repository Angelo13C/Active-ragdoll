using Unity.Entities;

public struct StrengthMultiplier : IComponentData
{
    public float ForceMultiplierOnCollision;
    public static StrengthMultiplier Invalid => new StrengthMultiplier { ForceMultiplierOnCollision = float.NaN };
    public bool IsValid => !float.IsNaN(ForceMultiplierOnCollision);

    public struct Root : IComponentData
    {
        public Entity RootEntity;

        public static bool operator==(Root a, Root b) => a.RootEntity == b.RootEntity;
        public static bool operator!=(Root a, Root b) => a.RootEntity != b.RootEntity;
        public override bool Equals(object obj) => obj is Root other && this == other;
        public override int GetHashCode() => RootEntity.GetHashCode();
    }
}