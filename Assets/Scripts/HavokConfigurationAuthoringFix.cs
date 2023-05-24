using System;
using Unity.Entities;
using UnityEngine;

namespace Havok.Physics.Authoring
{
    [AddComponentMenu("Entities/Physics/Havok Physics Configuration")]
    [DisallowMultipleComponent]
    public class HavokConfigurationAuthoringFix : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Allow dynamic rigid bodies to be excluded from simulation when they have come to rest. " +
            "This significantly improves the performance of scenes which have lots of settled dynamic bodies.")]
        public bool EnableSleeping = HavokConfiguration.Default.EnableSleeping != 0 ? true : false;

        [SerializeField]
        [Tooltip("The set of custom body tags which enable contact point welding in Havok. " +
            "Welding eliminates undesirable impulses when fast objects slide on or past other objects, with some additional performance cost.")]
        public Unity.Physics.Authoring.CustomPhysicsBodyTags BodyTagsForContactWelding;

        [Serializable]
        public class VisualDebuggerConfiguation
        {
            [SerializeField]
            [Tooltip("Enable profiling and debugging of the simulation via the Havok Visual Debugger application.")]
            public bool EnableVisualDebugger = HavokConfiguration.Default.VisualDebugger.Enable != 0 ? true : false;

            [SerializeField]
            [Tooltip("The port on which to send data to the Havok Visual Debugger application.")]
            public int Port = HavokConfiguration.Default.VisualDebugger.Port;

            [SerializeField]
            [Tooltip("The number of bytes to allocate per thread for collecting profiling information.")]
            public int TimerBufferSize = HavokConfiguration.Default.VisualDebugger.TimerBytesPerThread;
        }

        [SerializeField]
        public VisualDebuggerConfiguation VisualDebugger;

        // Return this as a HavokConfiguration component
        internal HavokConfiguration AsComponent => new HavokConfiguration
        {
            EnableSleeping = EnableSleeping ? 1 : 0,
            BodyTagsForContactWelding = BodyTagsForContactWelding.Value,
            VisualDebugger = new HavokConfiguration.VisualDebuggerConfiguration
            {
                Enable = VisualDebugger.EnableVisualDebugger ? 1 : 0,
                Port = VisualDebugger.Port,
                TimerBytesPerThread = VisualDebugger.TimerBufferSize
            }
        };

        class HavokConfigurationAuthoringBaker : Baker<HavokConfigurationAuthoringFix>
        {
            public override void Bake(HavokConfigurationAuthoringFix authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, authoring.AsComponent);
            }
        }
    }
}
