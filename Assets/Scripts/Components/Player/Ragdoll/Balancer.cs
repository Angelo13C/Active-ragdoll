using Unity.Entities;
using Unity.Mathematics;

public struct Balancer : IComponentData, IEnableableComponent
{
    public PolarCoordinates TargetAngle;
    public float Force;
    public BalanceAxis AxisType;
    // This is in degrees
    public float OffsetTargetYAngle;

    public enum BalanceAxis
    {
        Leg,
        Arm
    }
    
    public quaternion GetTargetRotationQuaternion()
    {
        var targetAngle = TargetAngle;
        targetAngle.Yaw += math.radians(OffsetTargetYAngle);
        return targetAngle.ToQuaternion(AxisType);
    }
}