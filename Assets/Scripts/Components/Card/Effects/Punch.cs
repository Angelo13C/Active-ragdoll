using Unity.Entities;
using Unity.Physics;

public struct Punch : IComponentData, IEnableableComponent
{
    public float StrengthMultiplier;
}