using Unity.Entities;

public struct BodyPartsReference : IComponentData
{
    public Entity Body;
    public Entity RightLowerArm;
}