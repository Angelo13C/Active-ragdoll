using Unity.Entities;
using Unity.Mathematics;

public struct Balancer : IComponentData, IEnableableComponent
{
    public PolarCoordinates TargetAngle;
    public float Force;
    // This is in degrees
    public float OffsetTargetYAngle;
    
    public quaternion GetTargetRotationQuaternion()
    {
        var targetAngle = TargetAngle;
        targetAngle.Yaw += math.radians(OffsetTargetYAngle);
        return targetAngle.ToQuaternion();
    }
}