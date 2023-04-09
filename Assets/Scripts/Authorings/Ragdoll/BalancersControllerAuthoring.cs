using Unity.Entities;
using UnityEngine;

public class BalancersControllerAuthoring : MonoBehaviour
{
    [SerializeField] private bool _enabled = true;

    class Baker : Baker<BalancersControllerAuthoring>
    {
        public override void Bake(BalancersControllerAuthoring authoring)
        {
            var balancersController = new BalancersController
            {
                YRotationOffset = authoring.transform.eulerAngles.y
            };
            AddComponent(balancersController);
            SetComponentEnabled<BalancersController>(GetEntity(authoring), authoring._enabled);

            var childrenBalancers = GetComponentsInChildren<BalancerAuthoring>();
            var controlledBalancers = AddBuffer<ControlledBalancer>();
            controlledBalancers.ResizeUninitialized(childrenBalancers.Length);
            for (var i = 0; i < childrenBalancers.Length; i++)
                controlledBalancers[i] = new ControlledBalancer { BalancerEntity = GetEntity(childrenBalancers[i]) };
        }
    }
}
