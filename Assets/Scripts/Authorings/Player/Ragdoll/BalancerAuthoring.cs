using System.Diagnostics;
using UnityEngine;
using Unity.Entities;

public class BalancerAuthoring : MonoBehaviour
{
    [SerializeField] private bool _enabled = true;

    [SerializeField] private PolarCoordinates _targetAngle;
    [SerializeField] private Balancer.BalanceAxis _balanceAxisType = Balancer.BalanceAxis.Arm;
    [SerializeField] private float _force;

    class Baker : Baker<BalancerAuthoring>
    {
        public override void Bake(BalancerAuthoring authoring)
        {
            var balancer = new Balancer
            {
                TargetAngle = authoring._targetAngle.ToRadians(),
                AxisType = authoring._balanceAxisType,
                Force = authoring._force
            };

            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, balancer);
            SetComponentEnabled<Balancer>(entity, authoring._enabled);
        }
    }
    
    [Conditional("UNITY_EDITOR")]
    private void OnDrawGizmos()
    {
        var length = GetComponent<MeshFilter>().sharedMesh.bounds.size.y * transform.lossyScale.y;
        var offset = (Quaternion) _targetAngle.ToRadians().ToQuaternion(_balanceAxisType) * (Vector3.right * length / 2);
        Gizmos.color = new Color32(150, 75, 0, 255);
        Gizmos.DrawLine(transform.position + offset, transform.position - offset);
    }
}
