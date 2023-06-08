using Unity.Collections;
using Unity.Entities;
using Unity.Physics;

public struct ChangeLinearDampingOnPunch : IComponentData
{
    public float NewLinearDrag;

    public struct WithLookup
    {
        public ChangeLinearDampingOnPunch ChangeLinearDampingOnPunch;
        public ComponentLookup<PhysicsDamping> DampingLookup;
        [ReadOnly] public ComponentLookup<BodyPartsReference> RagdollBodyReferenceLookup;
        public ComponentLookup<Stunned> StunnedLookup;

        public void ApplyIfRequired(RigidBody rigidbodyThatHits, RefRO<StrengthMultiplier.Root> hitEntityRoot)
        {
            if (Punch.IsRigidBodyPunching(rigidbodyThatHits) && hitEntityRoot.IsValid)
            {
                if (RagdollBodyReferenceLookup.TryGetComponent(hitEntityRoot.ValueRO.RootEntity, out var ragdollBodyReference))
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
                        stunned.ValueRW.ExtraTimeToWaitAfterMaxSpeedRemove = 1.5f;
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