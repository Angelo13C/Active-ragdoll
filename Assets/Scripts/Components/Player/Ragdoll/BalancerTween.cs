using Unity.Entities;
using Unity.Mathematics;

public struct BalancerTween : IComponentData, IEnableableComponent
{
    public float3 FromTargetAngle;
    public float3 ToTargetAngle;
    public float CurrentTime;
    public float Duration;
    public float Direction;

    public void Update(float deltaTime)
    {
        CurrentTime += Direction * deltaTime;
        if (CurrentTime > Duration)
        {
            CurrentTime = Duration - (CurrentTime - Duration);
            Direction = -1;
        }
        else if (CurrentTime < 0)
        {
            CurrentTime = math.abs(CurrentTime);
            Direction = 1;
        }
    }

    public float3 Sample() => math.lerp(FromTargetAngle, ToTargetAngle, CurrentTime / Duration);
    //Direction == -1 ? FromTargetAngle : ToTargetAngle;
}
