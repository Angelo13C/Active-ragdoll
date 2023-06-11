using Unity.Entities;
using Unity.Mathematics;

public struct ChangeStrengthMultiplierBasedOnSpeed : IComponentData
{
    public float MinSpeedSqr;
    public float MaxSpeedSqr;
    public float StrengthMultiplierAtMaxSpeed;

    // The percentage is not linear since I am using speed squared, I don't know if it feels better
    public float GetPercentage(float3 velocity)
    {
        var currentSpeedSqr = math.min(math.lengthsq(velocity), MaxSpeedSqr);
        if (currentSpeedSqr <= MinSpeedSqr)
            return 0f;
        return math.unlerp(0f, MaxSpeedSqr, currentSpeedSqr);
    }
}