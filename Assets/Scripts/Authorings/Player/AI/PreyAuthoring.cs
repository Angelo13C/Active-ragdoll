using Unity.Entities;
using UnityEngine;

public class PreyAuthoring : MonoBehaviour
{
	private enum Team
	{
		Ally,
		Enemy
	}
	[SerializeField] private Team _team;

	class Baker : Baker<PreyAuthoring>
	{
		public override void Bake(PreyAuthoring authoring)
		{
			var authoringEntity = GetEntity(authoring, TransformUsageFlags.None);
			
			AddComponent<Prey>(authoringEntity);
			
			if(authoring._team == Team.Ally)
				AddComponent<AllyTeam>(authoringEntity);
			else
				AddComponent<EnemyTeam>(authoringEntity);
		}
	}
}