using System;
using Unity.Mathematics;

//The angles are in radians
[Serializable]
public struct PolarCoordinates
{
    public float Yaw;
    public float Pitch;

    public quaternion ToQuaternion()
    {
        // I don't know why the Yaw needs to be reversed
        var rotation = quaternion.RotateY(-Yaw);
        rotation = math.mul(rotation, quaternion.RotateZ(Pitch));
        return rotation;
    }
}

public static class FabrikBonePolarCoordinatesExtensions
{
    public static PolarCoordinates ToPolarCoordinates(this FabrikBone bone)
    {
        return new PolarCoordinates
        {
            Yaw = math.atan2(bone.Direction.z, bone.Direction.x), 
            Pitch = math.asin(bone.Direction.y)
        };
    }
}