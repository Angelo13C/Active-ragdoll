using System;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public class BalancerAuthoring : MonoBehaviour
{
    [SerializeField] private bool _enabled = true;

    [SerializeField] private float3 _targetAngle;
    [SerializeField] private float _force;

    class Baker : Baker<BalancerAuthoring>
    {
        public override void Bake(BalancerAuthoring authoring)
        {
            var balancer = new Balancer
            {
                TargetAngle = authoring._targetAngle,
                ParentRotation = authoring.transform.parent.eulerAngles,
                Force = authoring._force
            };

            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, balancer);
            SetComponentEnabled<Balancer>(entity, authoring._enabled);
        }
    }

    private void OnDrawGizmos()
    {
        var length = GetComponent<MeshFilter>().sharedMesh.bounds.size.y * transform.lossyScale.y;
        var offset = Quaternion.Euler(_targetAngle) * new Vector3(0, length / 2f, 0);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position + offset, transform.position - offset);
    }
}
