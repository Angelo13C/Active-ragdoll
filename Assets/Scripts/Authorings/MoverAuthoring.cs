using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class MoverAuthoring : MonoBehaviour
{
    [SerializeField] private float _forwardForce = 1;
    [SerializeField] private float _backwardForce = 1;
    [SerializeField] private float _lateralForce = 1;

    class Baker : Baker<MoverAuthoring>
    {
        public override void Bake(MoverAuthoring authoring)
        {
            var mover = new Mover
            {
                ForwardForce = authoring._forwardForce,
                BackwardForce = authoring._backwardForce,
                LateralForce = authoring._lateralForce,
                LocalMoveDirection = float2.zero
            };

            AddComponent(mover);
        }
    }
}
