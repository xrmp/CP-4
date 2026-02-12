using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class ResourceAuthoring : MonoBehaviour
{
    public int resourceValue = 10;
}

public class ResourceBaker : Baker<ResourceAuthoring>
{
    public override void Bake(ResourceAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);

        AddComponent(entity, new LocalTransform
        {
            Position = authoring.transform.position,
            Rotation = quaternion.identity,
            Scale = 0.8f
        });

        AddComponent<ResourceTag>(entity);
    }
}