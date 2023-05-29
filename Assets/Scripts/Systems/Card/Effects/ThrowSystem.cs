using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct ThrowSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);
        var deltaTime = SystemAPI.Time.DeltaTime;
        foreach (var (cardUsedBy, entity) in SystemAPI.Query<CardUsedBy>().WithDisabled<Throw>().WithEntityAccess())
        {
            if (cardUsedBy.UseType != CardUsedBy.Use.None)
            {
                SystemAPI.SetComponentEnabled<Throw>(entity, true);
            }
        }
        
        foreach (var (throw_, cardUsedBy, entity) in SystemAPI.Query<Throw, CardUsedBy>().WithEntityAccess())
        {
            var throwCopy = throw_;

            //This justSpawned serves to avoid an error if the entity is spawned the same frame as it is thrown because of the ECB
            var justSpawned = false;
            if (throwCopy.ThrownEntity == Entity.Null)
            {
                justSpawned = true;
                
                throwCopy.ThrownEntity = entityCommandBuffer.Instantiate(throwCopy.PrefabToThrow);
                entityCommandBuffer.AddComponent(throwCopy.ThrownEntity, new Parent
                {
                    Value = SystemAPI.GetComponent<BodyPartsReference>(cardUsedBy.UsedBy).RightLowerArm
                });
                entityCommandBuffer.RemoveComponent<PhysicsWorldIndex>(throwCopy.ThrownEntity);
                throwCopy.RemainingTimeUntilThrow = throwCopy.ThrowCooldown;

                var animationsCollection = SystemAPI.GetSingleton<PlayerRagdollAnimationsCollection>();
                var playerAnimations = SystemAPI.GetBuffer<PlayedAnimation>(cardUsedBy.UsedBy);
                animationsCollection.PlayAnimation(animationsCollection.ThrowBone, playerAnimations);
            }

            throwCopy.RemainingTimeUntilThrow -= deltaTime;
            if (!justSpawned && throwCopy.RemainingTimeUntilThrow <= 0f)
            {
                var thrownPhysicsVelocity = SystemAPI.GetComponentRW<PhysicsVelocity>(throwCopy.ThrownEntity, false);
                var offsetAngle = SystemAPI.GetComponent<BalancersController>(cardUsedBy.UsedBy).YRotationOffset;
                var offsetRotation = quaternion.RotateY(math.radians(-offsetAngle));
                thrownPhysicsVelocity.ValueRW.Linear = math.mul(offsetRotation, math.forward()) * throwCopy.ThrowSpeed;

                var thrownLocalTransform = SystemAPI.GetComponentRW<LocalTransform>(throwCopy.ThrownEntity, false);
                thrownLocalTransform.ValueRW = LocalTransform.FromMatrix(SystemAPI.GetComponent<LocalToWorld>(throwCopy.ThrownEntity).Value);
                
                SystemAPI.SetComponentEnabled<Throw>(entity, false);
                entityCommandBuffer.RemoveComponent<Parent>(throwCopy.ThrownEntity);
                entityCommandBuffer.AddSharedComponent(throwCopy.ThrownEntity, new PhysicsWorldIndex());
                
                throwCopy.ThrownEntity = Entity.Null;
            }
            
            entityCommandBuffer.SetComponent(entity, throwCopy);
        }
        
        entityCommandBuffer.Playback(state.EntityManager);
    }
}