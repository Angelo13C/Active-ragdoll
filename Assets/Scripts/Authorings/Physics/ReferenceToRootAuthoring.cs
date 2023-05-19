using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class ReferenceToRootAuthoring : MonoBehaviour
{
	class Baker : Baker<ReferenceToRootAuthoring>
	{
		public override void Bake(ReferenceToRootAuthoring authoring)
		{
			var entity = GetEntity(authoring, TransformUsageFlags.None);

			var root = new StrengthMultiplier.Root
			{
				RootEntity = GetEntity(authoring.transform.root, TransformUsageFlags.None)
			};
			AddComponent(entity, root);
			
			var gameObjectsLinkedToRoot = authoring.transform.root.GetComponentsInChildren<Transform>();
			var linkedToRootBuffer = AddBuffer<StrengthMultiplierBakerSystem.LinkToRoot>(entity);
			linkedToRootBuffer.ResizeUninitialized(gameObjectsLinkedToRoot.Length);
			for (var i = 0; i < linkedToRootBuffer.Length; i++)
				linkedToRootBuffer[i] = GetEntity(gameObjectsLinkedToRoot[i], TransformUsageFlags.None);
		}
	}
}

	
[WorldSystemFilter(WorldSystemFilterFlags.BakingSystem)]
partial struct StrengthMultiplierBakerSystem : ISystem
{
	public struct LinkToRoot : IBufferElementData
	{
		public Entity RequiresRootComponent;
		
		public static implicit operator LinkToRoot(Entity entity) => new LinkToRoot { RequiresRootComponent = entity };
	}
	
	public void OnUpdate(ref SystemState state)
	{
		var entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);
		foreach (var (hasRoot, linksToRoot) in SystemAPI.Query<StrengthMultiplier.Root, DynamicBuffer<LinkToRoot>>())
		{
			foreach (var linkToRoot in linksToRoot)
			{
				entityCommandBuffer.AddComponent(linkToRoot.RequiresRootComponent, hasRoot);
			}
		}
		
		entityCommandBuffer.Playback(state.EntityManager);
	}
}