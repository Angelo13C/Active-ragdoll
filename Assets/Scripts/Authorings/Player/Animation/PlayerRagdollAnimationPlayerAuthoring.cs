using System.Linq;
using Unity.Entities;
using UnityEngine;

public class PlayerRagdollAnimationPlayerAuthoring : MonoBehaviour
{
	[SerializeField] private GameObject _leftArm, _rightArm;
	
	[Space]
	[SerializeField] private PlayerRagdollAnimationSO[] _initialAnimations = new PlayerRagdollAnimationSO[1];

	class Baker : Baker<PlayerRagdollAnimationPlayerAuthoring>
	{
		public override void Bake(PlayerRagdollAnimationPlayerAuthoring playerAuthoring)
		{
			var playerEntity = GetEntity(TransformUsageFlags.None);
			
			var animationPlayer = new PlayerRagdollAnimationPlayer
			{
				LeftArm = GetEntity(playerAuthoring._leftArm, TransformUsageFlags.None),
				RightArm = GetEntity(playerAuthoring._rightArm, TransformUsageFlags.None),
			};
			AddComponent(playerEntity, animationPlayer);

			var notNullInitialAnimationsCount = playerAuthoring._initialAnimations.Count(a => a != null);
			var playedAnimations = AddBuffer<PlayedAnimation>(playerEntity);
			playedAnimations.ResizeUninitialized(notNullInitialAnimationsCount);
			var j = 0;
			for (var i = 0; i < playedAnimations.Length; i++)
			{
				if (playerAuthoring._initialAnimations[i] != null)
				{
					playedAnimations[j] = new PlayedAnimation
					{
						Animation = playerAuthoring._initialAnimations[i].ToBlob(),
						CurrentTime = 0
					};
					j++;
				}
			}
		}
	}
}