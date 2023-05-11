using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct PlayerRagdollAnimation
{
    public bool Loop;
    public BlobArray<KeyFrame> KeyFrames;

    public KeyFrame? Sample(float time)
    {
        if (Loop && KeyFrames.Length > 0)
        {
            var lastKeyFrame = KeyFrames[KeyFrames.Length - 1];
            if(lastKeyFrame.Time != 0)
                time %= lastKeyFrame.Time;
        }
        
        for(var i = 0; i < KeyFrames.Length; i++)
        {
            if (KeyFrames[i].Time >= time)
            {
                return KeyFrames[math.max(i - 1, 0)];
            }
        }

        return null;
    }

    [System.Serializable]
    public struct KeyFrame
    {
        [Min(0)] public float Time;
        
        public ArmKey RightArmKey;
        public ArmKey LeftArmKey;

        [System.Serializable]
        public struct ArmKey
        {
            public bool Override;
            public float3 IKTargetPosition;
            public float3 IKPolePosition;
        }
    }
}