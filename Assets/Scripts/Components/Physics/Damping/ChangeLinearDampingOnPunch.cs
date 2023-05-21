using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using UnityEngine;

public struct ChangeLinearDampingOnPunch : IComponentData
{
    public float NewLinearDrag;

    public struct WithLookup
    {
        public ChangeLinearDampingOnPunch ChangeLinearDampingOnPunch;
        public ComponentLookup<PhysicsDamping> DampingLookup;
        [ReadOnly] public ComponentLookup<StrengthMultiplier.Root> RootLookup;
        [ReadOnly] public ComponentLookup<BodyPartsReference> RagdollBodyReferenceLookup;
        public ComponentLookup<Stunned> StunnedLookup;

        public void ApplyIfRequired(RigidBody rigidbodyThatHits, Entity entity)
        {
            if (Punch.IsRigidBodyPunching(rigidbodyThatHits))
            {
                if (RootLookup.TryGetComponent(entity, out var root))
                {
                    if (RagdollBodyReferenceLookup.TryGetComponent(root.RootEntity, out var ragdollBodyReference))
                    {
                        var damping = DampingLookup.GetRefRWOptional(ragdollBodyReference.Body, false);
                        if (damping.IsValid)
                            damping.ValueRW.Linear = ChangeLinearDampingOnPunch.NewLinearDrag;
                        
                        var stunned = StunnedLookup.GetRefRWOptional(ragdollBodyReference.Body, false);
                        if (stunned.IsValid)
                        {
                            StunnedLookup.SetComponentEnabled(ragdollBodyReference.Body, true);
                            stunned.ValueRW.Duration = 50f;
                            stunned.ValueRW.CompleteStun = true;
                        }
                    }
                }
            }
        }
    }
}

public struct BodyPartsReference : IComponentData
{
    public Entity Body;
    public Entity RightLowerArm;
}