using Unity.Entities;
using Unity.Mathematics;

public struct BalancerTween : IComponentData, IEnableableComponent
{
    public PolarCoordinates FromTargetAngle;
    public PolarCoordinates ToTargetAngle;
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

    public PolarCoordinates Sample() => math.lerp(FromTargetAngle, ToTargetAngle, CurrentTime / Duration);
}
