#if UNITY_EDITOR
using UnityEditor;
#endif
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu(fileName = " (Ragdoll Animation)", menuName = "Scriptable Objects/Ragdoll Animation")]
public class PlayerRagdollAnimationSO : ScriptableObject
{
    [SerializeField] private bool _loop;
    [SerializeField] [Min(0)] private float _lastFrameDuration;
#if UNITY_EDITOR
    [SerializeField] private int _currentAnimationFrameDebug;
#endif
    public PlayerRagdollAnimation.KeyFrame[] KeyFrames;

    public BlobAssetReference<PlayerRagdollAnimation> ToBlob()
    {
        var builder = new BlobBuilder(Allocator.Temp);
        ref var animation = ref builder.ConstructRoot<PlayerRagdollAnimation>();

        animation.Loop = _loop;
        animation.LastFrameDuration = _lastFrameDuration;

        var blobKeyFrames = builder.Allocate(ref animation.KeyFrames, KeyFrames.Length);
        for (var i = 0; i < blobKeyFrames.Length; i++)
            blobKeyFrames[i] = KeyFrames[i];

        var blob = builder.CreateBlobAssetReference<PlayerRagdollAnimation>(Allocator.Persistent);
        builder.Dispose();
        return blob;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        _currentAnimationFrameDebug = math.clamp(_currentAnimationFrameDebug, 0, KeyFrames.Length - 1);
    }

    [UnityEditor.CustomEditor(typeof(PlayerRagdollAnimationSO))]
    public class CustomEditor : Editor
    {
        private void OnEnable() => SceneView.duringSceneGui += OnSceneGUI;
        private void OnDisable() => SceneView.duringSceneGui -= OnSceneGUI;
 
        private void OnSceneGUI(SceneView sv)
        {
            var animationSO = (PlayerRagdollAnimationSO) target;
            if (animationSO.KeyFrames == null || animationSO.KeyFrames.Length == 0)
                return;
            
            var randomRagdoll = FindObjectOfType<PlayerRagdollAnimationPlayerAuthoring>();
            var fabrikSolvers = randomRagdoll.GetComponentsInChildren<FabrikSolverAuthoring>();
            var leftArmOffset = float3.zero;
            var rightArmOffset = float3.zero;
            foreach (var fabrikSolver in fabrikSolvers)
            {
                if(fabrikSolver.name.ToLower().Contains("left"))
                    leftArmOffset = fabrikSolver.GlobalOffset;
                else if(fabrikSolver.name.ToLower().Contains("right"))
                    rightArmOffset = fabrikSolver.GlobalOffset;
            }

            var animationBlob = animationSO.ToBlob();
            var keyFrame = animationBlob.Value.KeyFrames[animationSO._currentAnimationFrameDebug];

            void DoHandle(ref float3 position, float3 offset, Color color, string undoRecordName)
            {
                position = (float3) Handles.PositionHandle(position + offset, quaternion.identity) - offset;
                if (EditorGUI.EndChangeCheck())
                    Undo.RecordObject(animationSO, "Change " + undoRecordName + " Position");
                Handles.color = color;
                Handles.DrawWireCube(position + offset, new float3(0.2f, 0.2f, 0.2f));
            }

            if (keyFrame.LeftArmKey.Override)
            {
                DoHandle(ref keyFrame.LeftArmKey.IKTargetPosition, leftArmOffset, new Color(0, 1, 0), "Target");
                DoHandle(ref keyFrame.LeftArmKey.IKPolePosition, leftArmOffset, new Color(0.4f, 1, 0.4f), "Pole");
            }

            if (keyFrame.RightArmKey.Override)
            {
                DoHandle(ref keyFrame.RightArmKey.IKTargetPosition, rightArmOffset, new Color(0, 0, 1), "Target");
                DoHandle(ref keyFrame.RightArmKey.IKPolePosition, rightArmOffset, new Color(0.4f, 0.4f, 1), "Pole");
            }
            
            animationSO.KeyFrames[animationSO._currentAnimationFrameDebug] = keyFrame;
        }
    }

    [CustomPropertyDrawer(typeof(PlayerRagdollAnimation.KeyFrame.ArmKey))]
    public class ArmKeyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var height = EditorGUIUtility.singleLineHeight;
            var overrideProperty = property.FindPropertyRelative("Override");
            if (overrideProperty.boolValue)
                height += EditorGUIUtility.singleLineHeight * 3;

            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PrefixLabel(position, label);
            var overrideProperty = property.FindPropertyRelative("Override");

            var overrideRect = new Rect(position.width - 20, position.y, 20, 20);
            overrideProperty.boolValue = EditorGUI.Toggle(overrideRect, overrideProperty.boolValue);
            if (overrideProperty.boolValue)
            {
                EditorGUI.indentLevel++;
                var ikTargetPositionRect = new Rect(position);
                ikTargetPositionRect.yMin += EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(ikTargetPositionRect, property.FindPropertyRelative("IKTargetPosition"));
                ikTargetPositionRect.yMin += EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(ikTargetPositionRect, property.FindPropertyRelative("IKPolePosition"));
                EditorGUI.indentLevel--;
            }
        }
    }
#endif
}