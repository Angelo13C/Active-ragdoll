using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class SineIKTargetAuthoring : MonoBehaviour
{
    [SerializeField] private bool _enabled = true;

    [Space]
    [SerializeField] private float3 _start;
    [SerializeField] private float3 _end;
    [SerializeField] private bool _local;
    [SerializeField] private float _duration;
    [SerializeField] private bool _reversed;

    private SineIKTarget Bake()
    {
        return new SineIKTarget
        {
            Start = _start,
            End = _end,
            CurrentTime = _duration / 2,
            Duration = _duration,
            Direction = _reversed ? -1 : 1,
            Local = _local
        };
    }

    class Baker : Baker<SineIKTargetAuthoring>
    {
        public override void Bake(SineIKTargetAuthoring authoring)
        {
            var sineIKTarget = authoring.Bake();
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, sineIKTarget);
            SetComponentEnabled<SineIKTarget>(entity, authoring._enabled);
        }
    }

#if UNITY_EDITOR
    [SerializeField] private bool _preview;
    private float3? _targetPositionBeforePreview;
    private bool _wasLocalBeforePreview;
    private SineIKTarget _sineIKTarget;
    private float3 _globalOffset => _local ? (float3)transform.position : float3.zero;

    private void Update()
    {
        if (_preview)
        {
            if (TryGetComponent<FabrikSolverAuthoring>(out var ikSolver))
            {
                if (!_targetPositionBeforePreview.HasValue)
                {
                    _targetPositionBeforePreview = ikSolver.Target;
                    _wasLocalBeforePreview = ikSolver.Local;
                    _sineIKTarget = Bake();
                }

                ikSolver.Local = _local;
                _sineIKTarget.Update(Time.deltaTime);
                ikSolver.Target = _sineIKTarget.Sample();
            }
        }
        else
        {
            if (_targetPositionBeforePreview.HasValue)
            {
                if (TryGetComponent<FabrikSolverAuthoring>(out var ikSolver))
                {
                    ikSolver.Target = _targetPositionBeforePreview.Value;
                    ikSolver.Local = _wasLocalBeforePreview;
                }
                _targetPositionBeforePreview = null;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color32(255, 140, 0, 255);
        Gizmos.DrawSphere(_start + _globalOffset, 0.1f);
        Gizmos.DrawSphere(_end + _globalOffset, 0.1f);
    }

    [UnityEditor.CustomEditor(typeof(SineIKTargetAuthoring))]
    public class CustomEditor : Editor
    {
        public void OnSceneGUI()
        {
            var authoring = target as SineIKTargetAuthoring;
            authoring._start = (float3)Handles.PositionHandle(authoring._start + authoring._globalOffset, Quaternion.identity) - authoring._globalOffset;
            authoring._end = (float3)Handles.PositionHandle(authoring._end + authoring._globalOffset, Quaternion.identity) - authoring._globalOffset;
        }
    }
#endif
}
