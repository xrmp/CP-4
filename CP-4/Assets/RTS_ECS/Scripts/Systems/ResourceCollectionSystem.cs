using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(UnitMovementSystem))]
public partial struct ResourceCollectionSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
        float collectionDistance = 2f;

        // ПРОВЕРЯЕМ СЧЕТЧИК
        if (!SystemAPI.HasSingleton<ResourceCount>())
        {
            var counterEntity = ecb.CreateEntity();
            ecb.AddComponent(counterEntity, new ResourceCount { Value = 0 });
        }

        // СБОР РЕСУРСОВ
        foreach (var (unitTransform, unitTarget, unitEntity) in
                 SystemAPI.Query<RefRO<LocalTransform>, RefRW<TargetPosition>>()
                 .WithAll<UnitTag>()
                 .WithEntityAccess())
        {
            foreach (var (resourceTransform, resourceEntity) in
                     SystemAPI.Query<RefRO<LocalTransform>>()
                     .WithAll<ResourceTag>()
                     .WithEntityAccess())
            {
                float distance = math.distance(unitTransform.ValueRO.Position, resourceTransform.ValueRO.Position);

                if (distance < collectionDistance)
                {
                    // ДВИГАЕМСЯ К РЕСУРСУ
                    unitTarget.ValueRW.Value = resourceTransform.ValueRO.Position;

                    // УДАЛЯЕМ РЕСУРС
                    ecb.DestroyEntity(resourceEntity);

                    // УВЕЛИЧИВАЕМ СЧЕТЧИК
                    var counter = SystemAPI.GetSingletonRW<ResourceCount>();
                    counter.ValueRW.Value += 10;

                    break;
                }
            }
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}