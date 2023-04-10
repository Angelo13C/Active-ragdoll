using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class BalancerTweenAuthoring : MonoBehaviour
{
    [SerializeField] private bool _enabled = true;

    [SerializeField] private float3 _fromTargetAngle;
    [SerializeField] private float3 _toTargetAngle;
    [SerializeField] private float _duration;
    [SerializeField] private bool _reversed;

    class Baker : Baker<BalancerTweenAuthoring>
    {
        public override void Bake(BalancerTweenAuthoring authoring)
        {
            var balancerTween = new BalancerTween
            {
                FromTargetAngle = authoring._fromTargetAngle,
                ToTargetAngle = authoring._toTargetAngle,
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
