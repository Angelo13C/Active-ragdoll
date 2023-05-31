using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(ExplosiveAuthoring))]
public class ExplodeOnCollisionAuthoring : MonoBehaviour
{
	[SerializeField] private Vector3 _collisionNormal = Vector3.up;
	[SerializeField] private float _maxAngle = 180f;
	class Baker : Baker<ExplodeOnCollisionAuthoring>
	{
		public override void Bake(ExplodeOnCollisionAuthoring authoring)
		{
			var explodeOnCollision = new ExplodeOnCollision
			{
				CollisionNormal = authoring._collisionNormal.normalized,
				MaxAngleCosine = math.cos(math.radians(authoring._maxAngle / 2f))
			};
			AddComponent(GetEntity(authoring, TransformUsageFlags.None), explodeOnCollision);
		}
	}
}