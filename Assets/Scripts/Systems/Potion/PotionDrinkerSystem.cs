using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

[BurstCompile]
public partial struct PotionDrinkerSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var entityCommandBuffer = SystemAPI.GetSingleton<EndVariableRateSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged);
        var deltaTime = SystemAPI.Time.DeltaTime;
        foreach (var (potion, entity) in SystemAPI.Query<RefRW<Potion>>().WithEntityAccess())
        {
            potion.ValueRW.ApplyAfterThisSeconds -= deltaTime;
            if (potion.ValueRO.ApplyAfterThisSeconds <= 0f)
            {
                //This is to prevent that the potion is re-applied (because the ECB doesn't seem to playback at the end of the frame?)
                potion.ValueRW.ApplyAfterThisSeconds = float.PositiveInfinity;
                
                var potionParent = SystemAPI.GetComponent<Parent>(entity).Value;
                var userRoot = SystemAPI.GetComponent<StrengthMultiplier.Root>(potionParent);
                var drinkedPotions = SystemAPI.GetBuffer<DrinkedPotion>(userRoot.RootEntity);
                drinkedPotions.Add(new DrinkedPotion
                {
                    Potion = SystemAPI.GetComponent<OriginalPrefab>(entity).Prefab,
                    PotionType = potion.ValueRO.PotionType,
                    TimeLeft = potion.ValueRO.Duration,
                    WasDrunkThisFrame = true
                });
                
                entityCommandBuffer.DestroyEntity(entity);
            }
        }
    }
}