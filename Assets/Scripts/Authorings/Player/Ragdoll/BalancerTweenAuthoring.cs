using Unity.Entities;
using UnityEngine;

public class BalancerTweenAuthoring : MonoBehaviour
{
    [SerializeField] private bool _enabled = true;

    [SerializeField] private PolarCoordinates _fromTargetAngle;
    [SerializeField] private PolarCoordinates _toTargetAngle;
    [SerializeField] private float _duration;
    [SerializeField] private bool _reversed;

    class Baker : Baker<BalancerTweenAuthoring>
    {
        public override void Bake(BalancerTweenAuthoring authoring)
        {
            var balancerTween = new BalancerTween
            {
                FromTargetAngle = authoring._fromTargetAngle.ToRadians(),
                ToTargetAngle = authoring._toTargetAngle.ToRadians(),
                CurrentTime = authoring._duration / 2,
                Duration = authoring._duration,
                Direction = authoring._reversed ? -1 : 1
            };

            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, balancerTween);
            SetComponentEnabled<BalancerTween>(entity, authoring._enabled);
        }
    }
}
