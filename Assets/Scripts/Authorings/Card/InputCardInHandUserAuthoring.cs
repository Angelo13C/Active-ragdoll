using Unity.Entities;
using UnityEngine;

public class InputCardInHandUserAuthoring : MonoBehaviour
{
	[SerializeField] public KeyCode _leftCardKeyCode = KeyCode.Mouse0;
	[SerializeField] public KeyCode _rightCardKeyCode = KeyCode.Mouse1;

	class Baker : Baker<InputCardInHandUserAuthoring>
	{
		public override void Bake(InputCardInHandUserAuthoring authoring)
		{
			var entity = GetEntity(authoring, TransformUsageFlags.None);
			
			var inputCardInHandUser = new InputCardInHandUser
			{
				LeftCardKeyCode = authoring._leftCardKeyCode,
				RightCardKeyCode = authoring._rightCardKeyCode
			};
			AddComponent(entity, inputCardInHandUser);
			
			var cardInHandUser = new CardInHandUser
			{
				TryingToUse = CardInHandUser.CardAction.None
			};
			AddComponent(entity, cardInHandUser);
		}
	}
}