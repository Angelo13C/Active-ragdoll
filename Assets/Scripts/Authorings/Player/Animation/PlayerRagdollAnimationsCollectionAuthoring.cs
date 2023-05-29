using Unity.Entities;
using UnityEngine;

public class PlayerRagdollAnimationsCollectionAuthoring : MonoBehaviour
{
	[SerializeField] private PlayerRagdollAnimationSO _punch;
	[SerializeField] private PlayerRagdollAnimationSO _bowShot;
	[SerializeField] private PlayerRagdollAnimationSO _throwBone;

	class Baker : Baker<PlayerRagdollAnimationsCollectionAuthoring>
	{
		public override void Bake(PlayerRagdollAnimationsCollectionAuthoring authoring)
		{
			var animationsCollection = new PlayerRagdollAnimationsCollection
			{
				Punch = authoring._punch.ToBlob(),
				BowShot = authoring._bowShot.ToBlob(),
				ThrowBone = authoring._throwBone.ToBlob()
			};
			AddComponent(GetEntity(TransformUsageFlags.None), animationsCollection);
		}
	}
}