using Unity.Entities;

public struct StrengthMultiplier : IComponentData, IEnableableComponent
{
    public float ForceMultiplierOnCollision;

    public struct Root : IComponentData
    {
        public Entity RootEntity;

        public static bool operator==(Root a, Root b) => a.RootEntity == b.RootEntity;
        public static bool operator!=(Root a, Root b) => !(a == b);
        public override bool Equals(object obj) => obj is Root other && this == other;
        public override int GetHashCode() => RootEntity.GetHashCode();
    }

    public struct Timer : IBufferElementData
    {
        public Entity HitEntity;
        public float RemainingTime;
        
        public bool HasExpired => RemainingTime <= 0f;
    }
}

public static class StrengthMultiplierTimerExtensions
{
    public static bool Contains(this DynamicBuffer<StrengthMultiplier.Timer> timers, Entity entity)
    {
        foreach (var timer in timers)
        {
            if (timer.HitEntity == entity)
                return true;
        }

        return false;
    }
}