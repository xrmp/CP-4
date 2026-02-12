using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct UnitMovementSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;

        new UnitMovementJob
        {
            DeltaTime = deltaTime
        }.ScheduleParallel();
    }
}

[BurstCompile]
public partial struct UnitMovementJob : IJobEntity
{
    public float DeltaTime;

    public void Execute(ref LocalTransform transform, ref TargetPosition target, in MoveSpeed speed)
    {
        float3 direction = target.Value - transform.Position;
        float distance = math.length(direction);

        if (distance > 0.1f)
        {
            float3 move = math.normalizesafe(direction) * speed.Value * DeltaTime;

            // Не перелетаем цель
            if (math.length(move) > distance)
            {
                transform.Position = target.Value;
            }
            else
            {
                transform.Position += move;
            }

            // Поворот в сторону движения
            if (math.lengthsq(move) > 0.001f)
            {
                transform.Rotation = quaternion.LookRotationSafe(math.normalizesafe(direction), new float3(0, 1, 0));
            }
        }
    }
}