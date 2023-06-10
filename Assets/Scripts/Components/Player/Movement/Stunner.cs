using Unity.Entities;

public struct Stunner : IComponentData
{
    public float NewLinearDamping;
    public Stunned Stun;
}