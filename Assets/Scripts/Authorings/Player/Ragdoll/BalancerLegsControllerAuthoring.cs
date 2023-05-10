using Unity.Entities;
using UnityEngine;

public class BalancerLegsControllerAuthoring : MonoBehaviour
{
    [SerializeField] private BalancerTweenAuthoring[] _legs = new BalancerTweenAuthoring[0];

    class Baker : Baker<BalancerLegsControllerAuthoring>
    {
        public override void Bake(BalancerLegsControllerAuthoring authoring)
        {
            var controlledLegs = AddBuffer<ControlledBalancerLeg>(GetEntity(TransformUsageFlags.Dynamic));
            controlledLegs.ResizeUninitialized(authoring._legs.Length);
            var transformUsageFlags = TransformUsageFlags.Dynamic;
            for (var i = 0; i < authoring._legs.Length; i++)
            {
                controlledLegs[i] = new ControlledBalancerLeg
                {
                    Leg = GetEntity(authoring._legs[i], transformUsageFlags),
                    CurrentMoveDirection = ControlledBalancerLeg.MoveDirection.Forward
                };
                
            }
        }
    }
}
