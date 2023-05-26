using Unity.Entities;
using Unity.Mathematics;

public struct Bow : IComponentData
{
    public Entity ArrowPrefab;
    public Entity CurrentlyShootingArrow;
    
    public Entity UpperString;
    public Entity LowerString;

    public float3 MaxChargeStringPosition;
    public float3 MaxChargeStringOffset;
    public float CurrentChargePercentage;
    public float StringStretchAtMaxCharge;

    public State CurrentState;
    public float ChargeSpeed;

    public float ShootVelocity;
    
    private const float MAX_CHARGE_PERCENTAGE = 1f;
    public const float MIN_CHARGE_PERCENTAGE = 0f;

    public float3 CurrentStringTargetPosition => math.lerp(MaxChargeStringOffset, MaxChargeStringPosition, CurrentChargePercentage);
    public float CurrentStringStretch => math.lerp(1, StringStretchAtMaxCharge, CurrentChargePercentage);

    public void StartChargeArrow() => CurrentState = State.JustStartedCharging;
    public void ReleaseArrow() => CurrentState = State.JustReleased;

    public void Update(float deltaTime)
    {
        if (CurrentState == State.JustStartedCharging)
            CurrentState = State.Charge;
        else if (CurrentState == State.JustReleased)
            CurrentState = State.Release;
        
        if(CurrentState == State.Charge && CurrentChargePercentage == MAX_CHARGE_PERCENTAGE)
            ReleaseArrow();
        
        if (CurrentState == State.Charge)
        {
            CurrentChargePercentage += deltaTime * ChargeSpeed;
            CurrentChargePercentage = math.min(CurrentChargePercentage, MAX_CHARGE_PERCENTAGE);
        }
        else if(CurrentState == State.Release)
        {
            CurrentChargePercentage -= deltaTime * ChargeSpeed;
            if (CurrentChargePercentage <= MIN_CHARGE_PERCENTAGE)
            {
                CurrentChargePercentage = MIN_CHARGE_PERCENTAGE;
                CurrentState = State.Idle;
            }
        }
    }

    public enum State
    {
        Idle,
        JustStartedCharging,
        Charge,
        JustReleased,
        Release
    }
}