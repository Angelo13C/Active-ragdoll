using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class FirstPersonCameraAuthoring : MonoBehaviour
{
	[SerializeField] private GameObject _body;
	[SerializeField] private Vector3 _offset;
	
	[Header("Vertical rotation")]
	[SerializeField] private float _verticalSensitivity = 0.4f;
	[SerializeField] private float2 _verticalLimits = new float2(-40, 40);
	
	class Baker : Baker<FirstPersonCameraAuthoring>
	{
		public override void Bake(FirstPersonCameraAuthoring authoring)
		{
			Cursor.lockState = CursorLockMode.Locked;
			
			var offsetFromBody = authoring._offset;
			if (authoring._body != null)
				offsetFromBody += authoring.transform.position - authoring._body.transform.position;
			
			var cameraTarget = new FirstPersonCamera
			{
				CameraTransform = null,
				Body = GetEntity(authoring._body, TransformUsageFlags.Dynamic),
				OffsetFromBody = offsetFromBody,
				
				CurrentVerticalRotation = 0,
				VerticalRotationLimits = math.radians(authoring._verticalLimits),
				VerticalRotationSensitivity = authoring._verticalSensitivity
			};
			AddComponentObject(GetEntity(TransformUsageFlags.Dynamic), cameraTarget);
		}
	}
}