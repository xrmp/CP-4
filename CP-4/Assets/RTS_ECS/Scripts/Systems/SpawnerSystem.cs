using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial struct SpawnerSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BeginInitializationEntityCommandBufferSystem.Singleton>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        // 🔍 ПОИСК ПРЕФАБОВ
        Entity unitPrefab = Entity.Null;
        Entity resourcePrefab = Entity.Null;

        foreach (var (transform, tag, entity) in
                 SystemAPI.Query<LocalTransform, UnitTag>()
                 .WithOptions(EntityQueryOptions.IncludePrefab)
                 .WithEntityAccess())
        {
            unitPrefab = entity;
            Debug.Log($"✅ Найден Unit префаб: {entity}");
            break;
        }

        foreach (var (transform, tag, entity) in
                 SystemAPI.Query<LocalTransform, ResourceTag>()
                 .WithOptions(EntityQueryOptions.IncludePrefab)
                 .WithEntityAccess())
        {
            resourcePrefab = entity;
            Debug.Log($"✅ Найден Resource префаб: {entity}");
            break;
        }

        if (unitPrefab == Entity.Null || resourcePrefab == Entity.Null)
        {
            Debug.LogError("❌ ПРЕФАБЫ НЕ НАЙДЕНЫ! Добавь Unit.prefab и Resource.prefab на сцену и Apply Overrides!");
            state.Enabled = false;
            return;
        }

        // 🚀 СПАВН 5 ЮНИТОВ
        var units = new NativeArray<Entity>(5, Allocator.Temp);
        ecb.Instantiate(unitPrefab, units);

        for (int i = 0; i < units.Length; i++)
        {
            float3 position = new float3(i * 2 - 4, 0.5f, 0f);
            ecb.SetComponent(units[i], LocalTransform.FromPosition(position));
            ecb.SetComponent(units[i], new TargetPosition { Value = position });
        }

        // 🚀 СПАВН 10 РЕСУРСОВ
        var resources = new NativeArray<Entity>(10, Allocator.Temp);
        ecb.Instantiate(resourcePrefab, resources);

        for (int i = 0; i < resources.Length; i++)
        {
            float x = UnityEngine.Random.Range(-7f, 7f);
            float z = UnityEngine.Random.Range(2f, 8f);
            ecb.SetComponent(resources[i], LocalTransform.FromPosition(new float3(x, 0.5f, z)));
        }

        // 📊 СЧЕТЧИК
        var counterEntity = ecb.CreateEntity();
        ecb.AddComponent(counterEntity, new ResourceCount { Value = 0 });

        Debug.Log($"🎮 СОЗДАНО: {units.Length} юнитов и {resources.Length} ресурсов");

        state.Enabled = false;
    }
}