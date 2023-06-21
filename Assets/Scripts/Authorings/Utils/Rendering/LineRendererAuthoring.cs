using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[ExecuteInEditMode]
public class LineRendererAuthoring : MonoBehaviour
{
	[SerializeField] private float _lineWidth = 0.1f;
	public float LineWidth => _lineWidth;
	[SerializeField] [Range(0, 100)] private float _displayPercentage = MAX_DISPLAY_PERCENTAGE;
	public const float MAX_DISPLAY_PERCENTAGE = 100f;
	public float DisplayPercentage { get => _displayPercentage; set => _displayPercentage = Mathf.Min(MAX_DISPLAY_PERCENTAGE, value); }
	
	[SerializeField] private Material _material;
	[SerializeField] private float3[] _points = new float3[0];
	public float3[] Points => _points;

	public void SetPoints(NativeArray<float3> points) => _points = points.ToArray();

	private void Update()
	{
		var (lineRenderer, points, graphics) = Bake();
		graphics.Draw(lineRenderer, points, transform.position, transform.rotation, new float3(0, 0, -1));
	}

	private (LineRenderer, NativeArray<LineRenderer.Point>, LineRenderer.Graphics) Bake()
	{
		var lineRenderer = new LineRenderer
		{
			LineWidth = _lineWidth,
			DisplayPercentage = _displayPercentage / 100f
		};
		var points = new NativeArray<LineRenderer.Point>(_points.Length, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
		for (var i = 0; i < points.Length; i++)
			points[i] = new LineRenderer.Point { Position = _points[i] };

		var graphics = new LineRenderer.Graphics
		{
			Material = _material
		};
		
		return (lineRenderer, points, graphics);
	}

	class Baker : Baker<LineRendererAuthoring>
	{
		public override void Bake(LineRendererAuthoring authoring)
		{
			var (lineRenderer, points, graphics) = authoring.Bake();
			
			var entity = GetEntity(authoring, TransformUsageFlags.Renderable);
			AddComponent(entity, lineRenderer);
			var pointsBuffer = AddBuffer<LineRenderer.Point>(entity);
			pointsBuffer.CopyFrom(points);
			AddComponentObject(entity, graphics);
		}
	}
}