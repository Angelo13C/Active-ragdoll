using UnityEngine;
using Unity.Entities;

public class IconAuthoring : MonoBehaviour
{
    [SerializeField] private Sprite _sprite;
    
    class Baker : Baker<IconAuthoring>
    {
        public override void Bake(IconAuthoring authoring)
        {
            var icon = new Icon
            {
                Sprite = authoring._sprite
            };
            AddComponentObject(GetEntity(authoring, TransformUsageFlags.None), icon);
        }
    }
}