using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial class LineRendererSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var cameraPosition = float3.zero;
        foreach (var firstPersonCamera in SystemAPI.Query<FirstPersonCamera>())
        {
            if(firstPersonCamera.CameraTransform != null)
                cameraPosition = firstPersonCamera.CameraTransform.position;
        }
        
        foreach (var (lineRenderer, points, graphics, transform) in SystemAPI.Query<LineRenderer, DynamicBuffer<LineRenderer.Point>, LineRenderer.Graphics, LocalToWorld>())
        {
            // I don't think this is the right way to orient the line renderer toward the camera
            var watchPosition = cameraPosition - transform.Position;
            watchPosition.y = 0f;
            graphics.Draw(lineRenderer, points.AsNativeArray(), transform.Position, transform.Rotation, watchPosition);
        }
    }
}