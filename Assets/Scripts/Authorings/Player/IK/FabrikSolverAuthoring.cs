using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class FabrikSolverAuthoring : MonoBehaviour
{
    [Header("FABRIK Config")]
    [SerializeField] private int _maxIterationsCount = 40;
    [SerializeField] private float _maxErrorDistance = 0.01f;

    [Header("Data")]
    [SerializeField] private bool _local = true;
    public bool Local
    {
        get => _local;
        set => _local = value;
    }
    [SerializeField] private Vector3 _target;
    public Vector3 Target
    {
        get => _target;
        set => _target = value;
    }

    [SerializeField] private Vector3 _pole;

    private Vector3 GlobalOffset => _local ? transform.position : Vector3.zero;
    private Vector3 GlobalTarget => _target + GlobalOffset;
    private Vector3 GlobalPole => _pole + GlobalOffset;

    private NativeArray<FabrikBone> BakeBones(bool localOffset, Allocator allocator)
    {
        var offset = localOffset ? GlobalOffset : Vector3.zero;
        var bones = new NativeArray<FabrikBone>(transform.childCount, allocator, NativeArrayOptions.UninitializedMemory);
        if (bones.Length > 0)
        {
            for (var i = 0; i < bones.Length; i++)
            {
                var child = transform.GetChild(i);
                var childMesh = child.GetComponent<MeshFilter>().sharedMesh;
                var childSize = new Vector3(child.lossyScale.x * childMesh.bounds.extents.x, child.lossyScale.y * childMesh.bounds.extents.y, child.lossyScale.z * childMesh.bounds.extents.z);
                var biggestChildAxis = Vector3.zero;
                if (childSize.x > childSize.y && childSize.x > childSize.z)
                    biggestChildAxis.x = 1;
                else if (childSize.y > childSize.x && childSize.y > childSize.z)
                    biggestChildAxis.y = 1;
                else if (childSize.z > childSize.x && childSize.z > childSize.y)
                    biggestChildAxis.z = 1;
                var biggestAxisSize = Vector3.Dot(biggestChildAxis, childSize);
                var direction = child.rotation * biggestChildAxis;
                var start = child.position - offset + direction * biggestAxisSize;

                var overlappingLength = 0f;
                if (i != 0)
                {
                    overlappingLength = math.distance(bones[i - 1].End, start) / 2;
                    var previousBone = bones[i - 1];
                    previousBone.Length -= overlappingLength;
                    bones[i - 1] = previousBone;
                }

                bones[i] = new FabrikBone
                {
                    Start = start + direction * overlappingLength,
                    Direction = -direction,
                    Length = biggestAxisSize * 2 - overlappingLength
                };
            }
        }

        return bones;
    }

    class Baker : Baker<FabrikSolverAuthoring>
    {
        public override void Bake(FabrikSolverAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            
            var ikSolver = new IKSolver
            {
                Solver = new FabrikSolver(authoring._maxIterationsCount, authoring._maxErrorDistance),
                Local = authoring._local,
            };
            AddComponent(entity, ikSolver);

            var bones = authoring.BakeBones(false, Allocator.Temp);
            var bonesAndEntities = AddBuffer<IKBoneAndEntity>(entity);
            bonesAndEntities.ResizeUninitialized(bones.Length);
            for (var i = 0; i < bones.Length; i++)
            {
                // Why the child index needs to be reversed???
                var childIndex = bones.Length - 1 - i;
                bonesAndEntities[i] = new IKBoneAndEntity
                {
                    Bone = bones[i],
                    Entity = GetEntity(authoring.transform.GetChild(childIndex), TransformUsageFlags.Dynamic)
                };
            }
        }
    }

    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(GlobalTarget, 0.1f);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(GlobalPole, 0.1f);

        Gizmos.color = Color.magenta;
        var bones = BakeBones(true, Allocator.Temp);
        var fabrikSolver = new FabrikSolver(_maxIterationsCount, _maxErrorDistance);
        fabrikSolver.Solve(bones, _target, _pole);
        foreach (var bone in bones)
        {
            Gizmos.DrawLine(bone.Start + (float3)GlobalOffset, bone.End + (float3)GlobalOffset);
        }
    }

    [UnityEditor.CustomEditor(typeof(FabrikSolverAuthoring))]
    public class CustomEditor : Editor
    {
        public void OnSceneGUI()
        {
            var authoring = target as FabrikSolverAuthoring;
            authoring._target = Handles.PositionHandle(authoring.GlobalTarget, Quaternion.identity) - authoring.GlobalOffset;
            authoring._pole = Handles.PositionHandle(authoring.GlobalPole, Quaternion.identity) - authoring.GlobalOffset;
        }
    }

    #endif
}
