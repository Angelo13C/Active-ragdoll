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
        return animation.Sample(CurrentTime);
    }
}