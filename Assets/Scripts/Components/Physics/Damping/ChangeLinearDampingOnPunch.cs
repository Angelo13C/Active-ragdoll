using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using UnityEngine;

public struct ChangeLinearDampingOnPunch : IComponentData
{
    public float NewLinearDrag;
    public byte PunchCustomTag;

    public struct WithLookup
    {
        public ChangeLinearDampingOnPunch ChangeLinearDampingOnPunch;
        public ComponentLookup<PhysicsDamping> DampingLookup;
        [ReadOnly] public ComponentLookup<StrengthMultiplier.Root> RootLookup;
        [ReadOnly] public ComponentLookup<RagdollBodyReference> RagdollBodyReferenceLookup;
        public ComponentLookup<Stunned> StunnedLookup;

        public void ApplyIfRequired(RigidBody rigidbodyThatHits, Entity entity)
        {
            if ((rigidbodyThatHits.CustomTags & ChangeLinearDampingOnPunch.PunchCustomTag) != 0)
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

public struct RagdollBodyReference : IComponentData
{
    public Entity Body;
}