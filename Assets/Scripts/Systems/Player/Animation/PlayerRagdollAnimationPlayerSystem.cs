using Unity.Burst;
using Unity.Entities;

[BurstCompile]
public partial struct PlayerRagdollAnimationPlayerSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var deltaTime = SystemAPI.Time.DeltaTime;
        var ikSolverLookup = SystemAPI.GetComponentLookup<IKSolver>();
        foreach(var (animationPlayer, playedAnimations) in SystemAPI.Query<PlayerRagdollAnimationPlayer, DynamicBuffer<PlayedAnimation>>())
        {
            if (playedAnimations.Length > 0)
            {
                var leftArmIKSolver = ikSolverLookup.GetRefRWOptional(animationPlayer.LeftArm, false);
                var rightArmIKSolver = ikSolverLookup.GetRefRWOptional(animationPlayer.RightArm, false);
                
                for(var i = 0; i < playedAnimations.Length; i++)
                {
                    playedAnimations.ElementAt(i).CurrentTime += deltaTime;
                    var keyFrame = playedAnimations[i].Sample();
                    if (keyFrame.HasValue)
                    {
                        if (keyFrame.Value.LeftArmKey.Override && leftArmIKSolver.IsValid)
                        {
                            leftArmIKSolver.ValueRW.Target = keyFrame.Value.LeftArmKey.IKTargetPosition;
                            leftArmIKSolver.ValueRW.Pole = keyFrame.Value.LeftArmKey.IKPolePosition;
                        }

                        if (keyFrame.Value.RightArmKey.Override && rightArmIKSolver.IsValid)
                        {
                            rightArmIKSolver.ValueRW.Target = keyFrame.Value.RightArmKey.IKTargetPosition;
                            rightArmIKSolver.ValueRW.Pole = keyFrame.Value.RightArmKey.IKPolePosition;
                        }
                    }
                }
            }
        }
    }
}