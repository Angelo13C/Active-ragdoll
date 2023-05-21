using Unity.Entities;
using Unity.Physics;

public struct Punch : IComponentData, IEnableableComponent
{
    public float StrengthMultiplier;
    
    public const byte PUNCH_CUSTOM_PHYSICS_BODY_TAGS = 1;
    public static bool IsRigidBodyPunching(RigidBody body) => (body.CustomTags & PUNCH_CUSTOM_PHYSICS_BODY_TAGS) != 0;
}