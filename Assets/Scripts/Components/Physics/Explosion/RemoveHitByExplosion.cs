using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Authoring;

public struct RemoveHitByExplosion : IComponentData
{
    public CustomPhysicsBodyTags RemoveWhenCollidesWithTags;

    public bool CanRemove(RigidBody body) => (body.CustomTags & RemoveWhenCollidesWithTags.Value) != 0;
}