using UnityEngine;

public class PotionAuthoring : MonoBehaviour
{
	[SerializeField] private float _duration = 10f;

	public Potion BakePotion(PotionType potionType)
	{
		return new Potion
		{
			PotionType = potionType,
			Duration = _duration,
			ApplyAfterThisSeconds = Potion.APPLY_AFTER_THIS_SECONDS
		};
	}
}