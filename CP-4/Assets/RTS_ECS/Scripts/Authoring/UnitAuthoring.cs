using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class UnitAuthoring : MonoBehaviour
{
    public float moveSpeed = 3f;
}

public class UnitBaker : Baker<UnitAuthoring>
{
    public override void Bake(UnitAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);

        AddComponent(entity, new LocalTransform
        {
            Position = authoring.transform.position,
            Rotation = quaternion.identity,
            Scale = 1f
        });

        AddComponent(entity, new MoveSpeed { Value = authoring.moveSpeed });
        AddComponent(entity, new TargetPosition { Value = authoring.transform.position });
        AddComponent<UnitTag>(entity);
    }
}