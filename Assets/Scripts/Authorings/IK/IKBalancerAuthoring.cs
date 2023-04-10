using Unity.Entities;
using UnityEngine;

[RequireComponent(typeof(FabrikSolverAuthoring))]
public class IKBalancerAuthoring : MonoBehaviour
{
    class Baker : Baker<IKBalancerAuthoring>
    {
        public override void Bake(IKBalancerAuthoring authoring)
        {
            AddComponent<IKBalancer>(GetEntity(TransformUsageFlags.Dynamic));
        }
    }
}
