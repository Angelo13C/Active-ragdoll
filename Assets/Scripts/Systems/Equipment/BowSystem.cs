using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

[BurstCompile]
public partial struct BowSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var deltaTime = SystemAPI.Time.DeltaTime;
        var transformLookup = SystemAPI.GetComponentLookup<LocalTransform>();
        var postTransformMatrixLookup = SystemAPI.GetComponentLookup<PostTransformMatrix>();
        var entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (bow, cardUsedBy) in SystemAPI.Query<RefRW<Bow>, CardUsedBy>())
        {
            if (cardUsedBy.UseType != CardUsedBy.Use.None)
            {
                bow.ValueRW.StartChargeArrow();

                if (SystemAPI.HasBuffer<PlayedAnimation>(cardUsedBy.UsedBy))
                {
                    var playedAnimations = SystemAPI.GetBuffer<PlayedAnimation>(cardUsedBy.UsedBy);
                    var animationsCollection = SystemAPI.GetSingleton<PlayerRagdollAnimationsCollection>();
                    animationsCollection.PlayAnimation(animationsCollection.BowShot, playedAnimations);
                }
            }
        }
        
        foreach (var (bow, bowWorldTransform) in SystemAPI.Query<RefRW<Bow>, LocalToWorld>())
        {
            if (bow.ValueRO.CurrentState == Bow.State.JustStartedCharging)
            {
                bow.ValueRW.CurrentlyShootingArrow = state.EntityManager.Instantiate(bow.ValueRO.ArrowPrefab);
                entityCommandBuffer.RemoveComponent<PhysicsWorldIndex>(bow.ValueRO.CurrentlyShootingArrow);
            }
            else if (bow.ValueRO.CurrentState == Bow.State.JustReleased)
            {
                entityCommandBuffer.AddSharedComponent(bow.ValueRO.CurrentlyShootingArrow, new PhysicsWorldIndex(0));
                SystemAPI.SetComponent(bow.ValueRW.CurrentlyShootingArrow, new PhysicsVelocity
                {
                    Angular = float3.zero,
                    Linear = math.mul(bowWorldTransform.Rotation, bow.ValueRO.ShootVelocity * -math.right())
                });
                bow.ValueRW.CurrentlyShootingArrow = Entity.Null;
            }
            
            bow.ValueRW.Update(deltaTime);
            var targetStringPosition = bow.ValueRO.CurrentStringTargetPosition;

            if (bow.ValueRO.CurrentlyShootingArrow != Entity.Null)
            {
                var arrowTransform = transformLookup.GetRefRW(bow.ValueRW.CurrentlyShootingArrow, false);
                arrowTransform.ValueRW.Position = bowWorldTransform.Position + math.mul(bowWorldTransform.Rotation, targetStringPosition);
                arrowTransform.ValueRW.Rotation = bowWorldTransform.Rotation;
            }
            
            var upperStringTransform = transformLookup.GetRefRW(bow.ValueRO.UpperString, false);
            var lowerStringTransform = transformLookup.GetRefRW(bow.ValueRO.LowerString, false);

            var direction = targetStringPosition - lowerStringTransform.ValueRO.Position;
            var angle = math.atan2(direction.y, direction.x);
            angle = math.PI / 2 - angle;
            var targetUpperRotation = quaternion.RotateZ(angle);
            var targetLowerRotation = quaternion.RotateZ(-angle);
            
            upperStringTransform.ValueRW.Rotation = targetUpperRotation;
            lowerStringTransform.ValueRW.Rotation = targetLowerRotation;
            
            var upperStringScaleTransform = postTransformMatrixLookup.GetRefRW(bow.ValueRO.UpperString, false);
            var lowerStringScaleTransform = postTransformMatrixLookup.GetRefRW(bow.ValueRO.LowerString, false);
            var requiredYScale = bow.ValueRO.CurrentStringStretch;
            upperStringScaleTransform.ValueRW.Value = float4x4.Scale(1, requiredYScale, 1);
            lowerStringScaleTransform.ValueRW.Value = float4x4.Scale(1, -requiredYScale, 1);
        }
        
        entityCommandBuffer.Playback(state.EntityManager);
    }
}