using Unity.Entities;

public struct PlayerRagdollAnimationsCollection : IComponentData
{
    public BlobAssetReference<PlayerRagdollAnimation> Punch;
    public BlobAssetReference<PlayerRagdollAnimation> BowShot;
    
    public bool IsAnimationPlaying(BlobAssetReference<PlayerRagdollAnimation> animation, DynamicBuffer<PlayedAnimation> playedAnimations)
    {
        foreach (var playedAnimation in playedAnimations)
        {
            if (animation == playedAnimation.Animation)
                return true;
        }
        
        return false;
    }

    public void PlayAnimation(BlobAssetReference<PlayerRagdollAnimation> animation,
        DynamicBuffer<PlayedAnimation> playedAnimations)
    {
        playedAnimations.Add(new PlayedAnimation
        {
            CurrentTime = 0,
            Animation = animation
        });
    }
}