using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class BowAuthoring : MonoBehaviour
{
	[Header("Strings")]
	[SerializeField] private GameObject _upperString;
	[SerializeField] private GameObject _lowerString;
	
	[Header("Charge")]
	[SerializeField] private Vector3 _maxChargeStringPosition;
	[SerializeField] private Vector3 _maxChargeStringOffset;
	[SerializeField] [Min(0f)] private float _chargeTime = 0.2f;
	[SerializeField] [Min(0f)] private float _stringStretchAtMaxCharge = 1.3f;
	
	[Header("Shot")]
	[SerializeField] private GameObject _arrowPrefab;
	[SerializeField] [Min(0)] private float _shotVelocity = 20f;

	class Baker : Baker<BowAuthoring>
	{
		public override void Bake(BowAuthoring authoring)
		{
			var entity = GetEntity(authoring, TransformUsageFlags.None);
			
			var bow = new Bow
			{
				ArrowPrefab = GetEntity(authoring._arrowPrefab, TransformUsageFlags.None),
				UpperString = GetEntity(authoring._upperString, TransformUsageFlags.None),
				LowerString = GetEntity(authoring._lowerString, TransformUsageFlags.None),
				ChargeSpeed = 1f / authoring._chargeTime,
				CurrentChargePercentage = Bow.MIN_CHARGE_PERCENTAGE,
				CurrentState = Bow.State.Idle,
				MaxChargeStringPosition = authoring._maxChargeStringPosition,
				MaxChargeStringOffset = authoring._maxChargeStringOffset,
				StringStretchAtMaxCharge = authoring._stringStretchAtMaxCharge,
				ShootVelocity = authoring._shotVelocity,
				CurrentlyShootingArrow = Entity.Null
			};
			AddComponent(entity, bow);
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color32(150, 75, 0, 255);
		Gizmos.DrawWireSphere(_maxChargeStringPosition + transform.position, 0.1f);
	}

#if UNITY_EDITOR
	[UnityEditor.CustomEditor(typeof(BowAuthoring))]
	public class CustomEditor : Editor
	{
		public void OnSceneGUI()
		{
			var bowAuthoring = target as BowAuthoring;
			
			bowAuthoring._maxChargeStringPosition = Handles.PositionHandle(bowAuthoring._maxChargeStringPosition + bowAuthoring.transform.position, quaternion.identity) - bowAuthoring.transform.position;                if (EditorGUI.EndChangeCheck())
			if (EditorGUI.EndChangeCheck())
				Undo.RecordObject(bowAuthoring, "Change bow's string target position");
		}
	}
#endif
}