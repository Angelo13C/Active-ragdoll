using Unity.Entities;
using Unity.Mathematics;

public struct SineIKTarget : IComponentData, IEnableableComponent
{
    public float3 Start;
    public float3 End;
    public float CurrentTime;
    public float Duration;
    public float Direction;
    public bool Local;

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

    public float3 Sample() => math.lerp(Start, End, CurrentTime / Duration);
}
