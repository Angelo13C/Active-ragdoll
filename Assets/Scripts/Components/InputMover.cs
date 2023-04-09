using Unity.Entities;
using UnityEngine;

public struct InputMover : IComponentData
{
    public KeyCode ForwardKey;
    public KeyCode BackwardKey;
    public KeyCode RightKey;
    public KeyCode LeftKey;
}