using Unity.Entities;

public struct PlayerRagdollAnimationsCollection : IComponentData
{
    public BlobAssetReference<PlayerRagdollAnimation> Punch;

    public bool PlayAnimationIfNotYetPlayed(BlobAssetReference<PlayerRagdollAnimation> animation,
        DynamicBuffer<PlayedAnimation> playedAnimations)
    {
        foreach (var playedAnimation in playedAnimations)
        {
            if (animation == playedAnimation.Animation)
                return false;
        }

        playedAnimations.Add(new PlayedAnimation
        {
            CurrentTime = 0,
            Animation = animation
        });
        return true;
    }
}