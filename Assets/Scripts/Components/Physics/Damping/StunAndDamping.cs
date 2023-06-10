using Unity.Collections;
using Unity.Entities;
using Unity.Physics;

public struct StunAndDamping : IComponentData
{
    public ComponentLookup<PhysicsDamping> DampingLookup;
    [ReadOnly] public ComponentLookup<BodyPartsReference> RagdollBodyReferenceLookup;
    public ComponentLookup<Stunned> StunnedLookup;

    public void Apply(Entity hitEntity, float newLinearDamping, Stunned stun)
    {
        if (RagdollBodyReferenceLookup.TryGetComponent(hitEntity, out var ragdollBodyReference))
        {
            var damping = DampingLookup.GetRefRWOptional(ragdollBodyReference.Body, false);
            if (damping.IsValid)
                damping.ValueRW.Linear = newLinearDamping;

            var stunned = StunnedLookup.GetRefRWOptional(ragdollBodyReference.Body, false);
            if (stunned.IsValid)
            {
                StunnedLookup.SetComponentEnabled(ragdollBodyReference.Body, true);
                stunned.ValueRW = stun;
            }
        }
    }
}