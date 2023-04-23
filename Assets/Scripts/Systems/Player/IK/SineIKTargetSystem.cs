using Unity.Burst;
using Unity.Entities;

[BurstCompile]
public partial struct SineIKTargetSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var deltaTime = SystemAPI.Time.DeltaTime;
        foreach (var(sineIKTarget, ikSolver) in SystemAPI.Query<RefRW<SineIKTarget>, RefRW<IKSolver>>())
        {
            sineIKTarget.ValueRW.Update(deltaTime);
            ikSolver.ValueRW.Target = sineIKTarget.ValueRO.Sample();
            ikSolver.ValueRW.Local = sineIKTarget.ValueRO.Local;
        }
    }
}
