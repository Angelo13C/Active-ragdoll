using Unity.Entities;
using Unity.Mathematics;

public struct PlayerRagdollAnimationPlayer : IComponentData
{
    public Entity LeftArm;
    public Entity RightArm;
}

public struct PlayedAnimation : IBufferElementData
{
    public BlobAssetReference<PlayerRagdollAnimation> Animation;
    public float CurrentTime;

    public PlayerRagdollAnimation.KeyFrame? Sample()
    {
        ref var animation = ref Animation.Value;
        return animation.Sample(CurrentTime, out var _);
    }
    
    public bool HasFinished()
    {
        ref var animation = ref Animation.Value;
        if (animation.Loop)
            return false;
        
        return animation.KeyFrames.Length == 0 || CurrentTime > animation.KeyFrames[animation.KeyFrames.Length - 1].Time;
    }
}