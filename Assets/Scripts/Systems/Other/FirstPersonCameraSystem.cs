using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(PresentationSystemGroup))]
public partial class FirstPersonCameraSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var balancersControllerLookup = SystemAPI.GetComponentLookup<BalancersController>();
        foreach(var firstPersonCamera in SystemAPI.Query<FirstPersonCamera>())
        {
            if (firstPersonCamera.CameraTransform == null)
            {
                var camera = Camera.main;
                if (camera != null)
                    firstPersonCamera.CameraTransform = camera.transform;
            }

            if (firstPersonCamera.CameraTransform != null)
            {
                if (firstPersonCamera.Body != Entity.Null)
                {
                    var bodyTransform = SystemAPI.GetComponent<LocalToWorld>(firstPersonCamera.Body);
                    firstPersonCamera.CameraTransform.position = bodyTransform.Position + firstPersonCamera.OffsetFromBody;
                    
                    var bodyRotator = SystemAPI.GetComponent<Rotator>(firstPersonCamera.Body);
                    if (balancersControllerLookup.TryGetComponent(bodyRotator.BalancersControllerEntity, out var balancersController))
                    {
                        var deltaVerticalRotation = Input.GetAxis("Mouse Y") * firstPersonCamera.VerticalRotationSensitivity;
                        firstPersonCamera.CurrentVerticalRotation = math.clamp(firstPersonCamera.CurrentVerticalRotation + deltaVerticalRotation,
                            firstPersonCamera.VerticalRotationLimits.x, firstPersonCamera.VerticalRotationLimits.y);
                        
                        var rotation = quaternion.EulerXYZ(firstPersonCamera.CurrentVerticalRotation, -math.radians(balancersController.YRotationOffset), 0);
                        firstPersonCamera.CameraTransform.rotation = rotation;
                    }
                }
            }
        }
    }
}