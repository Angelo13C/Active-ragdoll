using Unity.Entities;
using UnityEngine;

public class PlayerRagdollAnimationsCollectionAuthoring : MonoBehaviour
{
	[SerializeField] private PlayerRagdollAnimationSO _punch;
	[SerializeField] private PlayerRagdollAnimationSO _bowShot;

	class Baker : Baker<PlayerRagdollAnimationsCollectionAuthoring>
	{
		public override void Bake(PlayerRagdollAnimationsCollectionAuthoring authoring)
		{
			var animationsCollection = new PlayerRagdollAnimationsCollection
			{
				Punch = authoring._punch.ToBlob(),
				BowShot = authoring._bowShot.ToBlob()
			};
			AddComponent(GetEntity(TransformUsageFlags.None), animationsCollection);
		}
	}
}