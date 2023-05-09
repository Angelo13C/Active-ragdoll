using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct PlayerRagdollAnimation
{
    public BlobArray<KeyFrame> KeyFrames;

    public KeyFrame? Sample(float time)
    {
        for(var i = 0; i < KeyFrames.Length; i++)
        {
            if (KeyFrames[i].Time >= time)
                time -= KeyFrames[i].Time;
            else
            {
                return KeyFrames[i];
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