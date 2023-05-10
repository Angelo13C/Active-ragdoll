using System;
using Unity.Mathematics;

//The angles are in radians
[Serializable]
public struct PolarCoordinates
{
    public float Yaw;
    public float Pitch;

    public quaternion ToQuaternion(Balancer.BalanceAxis axisType)
    {
        // I don't know why the Yaw needs to be reversed
        var rotation = quaternion.RotateY(-Yaw);
        if (axisType == Balancer.BalanceAxis.Arm)
            rotation = math.mul(rotation, quaternion.RotateZ(Pitch));
        else
            rotation = math.mul(rotation, quaternion.RotateX(Pitch));
        return rotation;
    }
    
    public static implicit operator float2(PolarCoordinates coordinates) => new float2(coordinates.Yaw, coordinates.Pitch);
    public static implicit operator PolarCoordinates(float2 coordinates) => new PolarCoordinates
    {
        Yaw = coordinates.x,
        Pitch = coordinates.y
    };

    public static PolarCoordinates operator -(PolarCoordinates coordinates) => -((float2) coordinates);

    public PolarCoordinates ToRadians() => math.radians(this);
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