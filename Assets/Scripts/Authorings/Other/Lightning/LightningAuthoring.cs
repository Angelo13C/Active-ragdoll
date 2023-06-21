using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;
#if UNITY_EDITOR
using Unity.Collections;
using UnityEditor;
#endif

[RequireComponent(typeof(LineRendererAuthoring))]
[ExecuteInEditMode]
public class LightningAuthoring : MonoBehaviour
{
	[SerializeField] private float _speedInPointsPerSecond = 200f;
	[SerializeField] private bool _triggerOnCreation = true;
	
	[Header("Disappear")]
	[SerializeField] private float _startTimeToDisappear = 0.4f;
	[SerializeField] private float _disappearDuration = 0.1f;
	
	[Header("Main bolt")]
	[SerializeField] [Min(0f)] private float _height = 150f;
	[SerializeField] [Min(0f)] private float _maxSpreadWidth = 1f;
	[SerializeField] private Vector2 _segmentLengthRange = new Vector2(1f, 4f);

	[Header("Sub bolts")]
	[SerializeField] private GameObject _subBoltPrefab;
	[SerializeField] private Vector2Int _subBoltsCountRange = new Vector2Int(3, 7);
	[SerializeField] private Vector2 _subBoltsHeightRange = new Vector2(10, 30);
	[SerializeField] private Vector2 _subBoltsWidthSpreadRange = new Vector2(0.5f, 1);
	[SerializeField] private Vector2 _subBoltsSegmentLengthRange = new Vector2(1, 2f);

	private (LightningBolt, LightningSubBoltsGenerator, LightningBoltDisappear) Bake(Entity subBoltPrefab)
	{
		return (new LightningBolt
		{
			Height = _height,
			MaxSpreadWidth = _maxSpreadWidth,
			SegmentLengthRange = _segmentLengthRange,
			SpeedInPointsPerSecond = _speedInPointsPerSecond
		}, new LightningSubBoltsGenerator
		{
			SubBoltPrefab = subBoltPrefab,
			SubBoltsCountRange = new int2(_subBoltsCountRange.x, _subBoltsCountRange.y),
			SubBoltsHeightRange = _subBoltsHeightRange,
			SubBoltsSegmentLengthRange = _subBoltsSegmentLengthRange,
			SubBoltsWidthSpreadRange = _subBoltsWidthSpreadRange
		}, new LightningBoltDisappear
		{
			CurrentTime = 0f,
			StartTimeToDisappear = _startTimeToDisappear,
			DisappearDuration = _disappearDuration,
			MaxWidth = GetComponent<LineRendererAuthoring>().LineWidth
		});
	}

	class Baker : Baker<LightningAuthoring>
	{
		public override void Bake(LightningAuthoring authoring)
		{
			var subBoltPrefab = GetEntity(authoring._subBoltPrefab, TransformUsageFlags.None);
			var (lightningBolt, lightningSubBoltGenerator, lightningBoltDisappear) = authoring.Bake(subBoltPrefab);
			var entity = GetEntity(authoring, TransformUsageFlags.None);
			AddComponent(entity, lightningBolt);
			AddComponent(entity, lightningSubBoltGenerator);
			AddComponent(entity, lightningBoltDisappear);
			AddComponent<LightningTrigger>(entity);
			SetComponentEnabled<LightningTrigger>(entity, authoring._triggerOnCreation);
		}
	}
	
#if UNITY_EDITOR
	private bool _shouldUpdate = false;
	
	private void Update()
	{
		if (!_shouldUpdate)
			return;
		
		var deltaPercentage = _speedInPointsPerSecond * Time.deltaTime;
		var lineRenderer = GetComponent<LineRendererAuthoring>();
		lineRenderer.DisplayPercentage += deltaPercentage;
		var lowestVisiblePointIndex = Math.Ceiling(lineRenderer.DisplayPercentage / 100f * (lineRenderer.Points.Length - 1));
		var lowestVisiblePointY = lineRenderer.Points[(int) lowestVisiblePointIndex].y;
		for(var i = 0; i < transform.childCount; i++)
		{
			var subBolt = transform.GetChild(i);
			if (subBolt.localPosition.y >= lowestVisiblePointY)
				subBolt.GetComponent<LineRendererAuthoring>().DisplayPercentage += deltaPercentage;
		}

		if (lineRenderer.DisplayPercentage >= LineRendererAuthoring.MAX_DISPLAY_PERCENTAGE)
			_shouldUpdate = false;
	}

	[CustomEditor(typeof(LightningAuthoring))]
	[CanEditMultipleObjects]
	public class AuthoringEditor : Editor 
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			if (GUILayout.Button("Generate lightning"))
			{
				var authoring = (LightningAuthoring) target;
				authoring._shouldUpdate = true;
				var rng = new Random((uint)UnityEngine.Random.Range(1, 99999));
				var (mainBolt, subBoltsGenerator, _) = authoring.Bake(Entity.Null);
				
				var lineRendererAuthoring = authoring.GetComponent<LineRendererAuthoring>();
				var mainBoltPoints = mainBolt.Generate(rng, Allocator.Temp);
				lineRendererAuthoring.SetPoints(mainBoltPoints);
				lineRendererAuthoring.DisplayPercentage = 0f;

				for(var i = authoring.transform.childCount - 1; i >= 0; i--)
					DestroyImmediate(authoring.transform.GetChild(i).gameObject);
				
				var subBolts = subBoltsGenerator.Generate(mainBoltPoints, rng, Allocator.Temp);
				for(var i = 0; i < subBolts.SubBoltsCount; i++)
				{
					var (subBoltPoints, subBoltTransform) = subBolts.GetNthSubBolt(i);
					var subBolt = Instantiate(authoring._subBoltPrefab, authoring.transform).transform;
					subBolt.SetLocalPositionAndRotation(subBoltTransform.Translation(), subBoltTransform.Rotation());
					var subLineRendererAuthoring = subBolt.GetComponent<LineRendererAuthoring>();
					subLineRendererAuthoring.SetPoints(subBoltPoints);
					subLineRendererAuthoring.DisplayPercentage = 0f;
				}
			}
		}
	}
#endif
}