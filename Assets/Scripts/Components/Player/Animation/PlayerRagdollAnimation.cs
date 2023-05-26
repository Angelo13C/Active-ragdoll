using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct PlayerRagdollAnimation
{
    public bool Loop;
    public BlobArray<KeyFrame> KeyFrames;
    public float LastFrameDuration;

    public float Duration => KeyFrames.Length == 0 ? 0f : KeyFrames[^1].Time + LastFrameDuration;

    public void SampleTimeAndPercentage(float time, out int index, out float percentage)
    {
        if (Loop && Duration != 0)
            time %= Duration;

        if (KeyFrames.Length == 1 && KeyFrames[0].Time <= time)
            time = KeyFrames[0].Time;
            
        for(index = 0; index < KeyFrames.Length - 1; index++)
        {
            if (KeyFrames[index + 1].Time >= time)
            {
                percentage = math.unlerp(KeyFrames[index].Time, KeyFrames[index + 1].Time, time);
                return;
            }
            else if (index + 1 == KeyFrames.Length - 1 && time <= Duration)
            {
                index++;
                percentage = 1;
                return;
            }
        }

        index = -1;
        percentage = -1;
    }

    // This doesn't work fine for keyframes where arms are not overriden!
    public KeyFrame? Sample(float time, out int index)
    {
        if (KeyFrames.Length == 1)
        {
            index = 0;
            return KeyFrames[0];
        }
            
        SampleTimeAndPercentage(time, out index, out var percentage);
        if (index == -1)
            return null;

        if (index == KeyFrames.Length - 1)
            return KeyFrames[index];
        
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