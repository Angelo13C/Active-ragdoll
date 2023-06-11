using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Authoring;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class StrengthMultiplierAuthoring : MonoBehaviour
{
	[SerializeField] [Min(0)] private float _forceMultiplierOnCollision = 1000f;

	[SerializeField] private bool _initiallyActive = false;

	private const CollisionResponsePolicy RAISE_EVENT = CollisionResponsePolicy.CollideRaiseCollisionEvents;
	
	class Baker : Baker<StrengthMultiplierAuthoring>
	{
		public override void Bake(StrengthMultiplierAuthoring authoring)
		{
			var entity = GetEntity(TransformUsageFlags.None);
			
			GetComponent<PhysicsShapeAuthoring>(authoring).CollisionResponse = RAISE_EVENT;
			var strengthMultiplier = new StrengthMultiplier
			{
				ForceMultiplierOnCollision = authoring._forceMultiplierOnCollision
			};
			AddComponent(entity, strengthMultiplier);
			SetComponentEnabled<StrengthMultiplier>(entity, authoring._initiallyActive);
			AddBuffer<StrengthMultiplier.Timer>(entity);
		}
	}
	
#if UNITY_EDITOR
	[CustomEditor(typeof(StrengthMultiplierAuthoring))]
	public class AuthoringEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			
			var authoring = (StrengthMultiplierAuthoring)target;
			if (authoring.GetComponent<PhysicsShapeAuthoring>().CollisionResponse != RAISE_EVENT)
			{
				EditorGUILayout.HelpBox("The `Collision Response` in the `Physics Shape` component needs to be set to " + RAISE_EVENT + " to make this work.\nIt will be automatically set by this script at run", MessageType.Warning);
			}
		}
	}
#endif
}