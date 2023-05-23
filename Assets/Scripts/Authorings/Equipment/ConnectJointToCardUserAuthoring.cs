using Unity.Entities;
using UnityEngine;

public class ConnectJointToCardUserAuthoring : MonoBehaviour
{
	class Baker : Baker<ConnectJointToCardUserAuthoring>
	{
		public override void Bake(ConnectJointToCardUserAuthoring authoring)
		{
			AddComponent<ConnectJointToCardUser>(GetEntity(authoring, TransformUsageFlags.None));
		}
	}
}