using Unity.Entities;
using UnityEngine;

public class InputMoverAuthoring : MonoBehaviour
{
	[SerializeField] private KeyCode _forwardKey = KeyCode.W;
	[SerializeField] private KeyCode _backwardKey = KeyCode.S;
	[SerializeField] private KeyCode _rightKey = KeyCode.D;
	[SerializeField] private KeyCode _leftKey = KeyCode.A;

	class Baker : Baker<InputMoverAuthoring>
	{
		public override void Bake(InputMoverAuthoring authoring)
		{
			var inputMover = new InputMover
			{
				ForwardKey = authoring._forwardKey,
				BackwardKey = authoring._backwardKey,
				RightKey = authoring._rightKey,
				LeftKey = authoring._leftKey,
			};
			AddComponent(inputMover);
		}
	}
}