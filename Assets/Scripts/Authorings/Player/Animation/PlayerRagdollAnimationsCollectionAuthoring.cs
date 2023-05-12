using Unity.Entities;
using UnityEngine;

public class PlayerRagdollAnimationsCollectionAuthoring : MonoBehaviour
{
	[SerializeField] private PlayerRagdollAnimationSO _punch;

	class Baker : Baker<PlayerRagdollAnimationsCollectionAuthoring>
	{
		public override void Bake(PlayerRagdollAnimationsCollectionAuthoring authoring)
		{
			var animationsCollection = new PlayerRagdollAnimationsCollection
			{
				Punch = authoring._punch.ToBlob()
			};
			AddComponent(GetEntity(TransformUsageFlags.None), animationsCollection);
		}
	}
}