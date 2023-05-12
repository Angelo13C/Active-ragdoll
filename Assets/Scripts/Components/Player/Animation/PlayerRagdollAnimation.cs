using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct PlayerRagdollAnimation
{
    public bool Loop;
    public BlobArray<KeyFrame> KeyFrames;

    public void SampleTimeAndPercentage(float time, out int index, out float percentage)
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
                index = math.max(i - 1, 0);
                percentage = math.unlerp(KeyFrames[index].Time, KeyFrames[i].Time, time);
                return;
            }
        }

        index = -1;
        percentage = -1;
    }

    public KeyFrame? SampleUnlerped(float time, out int index)
    {
        SampleTimeAndPercentage(time, out index, out var _);
        return index == -1 ? null : KeyFrames[index];
    }

    // This doesn't work fine for keyframes where arms are not overriden!
    public KeyFrame? Sample(float time, out int index)
    {
        SampleTimeAndPercentage(time, out index, out var percentage);
        if (index == -1)
            return null;

        return new KeyFrame
        {
            Time = time,
            LeftArmKey = KeyFrame.ArmKey.Lerp(KeyFrames[index].LeftArmKey, KeyFrames[index + 1].LeftArmKey, percentage),
            RightArmKey = KeyFrame.ArmKey.Lerp(KeyFrames[index].RightArmKey, KeyFrames[index + 1].RightArmKey,
                percentage)
        };
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

            public static ArmKey Lerp(ArmKey start, ArmKey end, float t)
            {
                return new ArmKey
                {
                    Override = start.Override || end.Override,
                    IKTargetPosition = math.lerp(start.IKTargetPosition, end.IKTargetPosition, t),
                    IKPolePosition = math.lerp(start.IKPolePosition, end.IKPolePosition, t)
                };
            }
        }
    }
}