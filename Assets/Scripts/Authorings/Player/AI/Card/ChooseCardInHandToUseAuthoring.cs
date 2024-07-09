using Unity.Entities;
using UnityEngine;

public class ChooseCardInHandToUseAuthoring : MonoBehaviour
{
	class Baker : Baker<ChooseCardInHandToUseAuthoring>
	{
		public override void Bake(ChooseCardInHandToUseAuthoring authoring)
		{
			var entity = GetEntity(authoring, TransformUsageFlags.None);
			
			var chooseCardInHandToUse = new ChooseCardInHandToUse
			{
				LeftCardPercentage = 0f,
				RightCardPercentage = 0f
			};
			AddComponent(entity, chooseCardInHandToUse);

			var cardInHandUser = new CardInHandUser
			{
				TryingToUse = CardInHandUser.CardAction.None
			};
			AddComponent(entity, cardInHandUser);
		}
	}
}