using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using Unity.Jobs;

public class GridSystem : JobComponentSystem
{
    EndInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem;

    protected override void OnCreateManager()
    {
        // Cache the EndSimulationBarrier in a field, so we don't have to create it every frame
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<EndInitializationEntityCommandBufferSystem>();
    }

    struct TileSpawnJob : IJobForEachWithEntity<GridSpawning, LocalToWorld>
    {
        public EntityCommandBuffer CommandBuffer;

        public void Execute(Entity entity, int index, [ReadOnly] ref GridSpawning spawner,
            [ReadOnly] ref LocalToWorld location)
        {
            for (int x = 0; x < spawner.CountX; x++)
            {
                for (int y = 0; y < spawner.CountZ; y++)
                {
                    var instance = CommandBuffer.Instantiate(spawner.Prefab);

                    // Place the instantiated in a grid with some noise
                    var position = new float3((float)x, 0, (float)y);
                    CommandBuffer.SetComponent(instance, new Translation { Value = position });
                }
            }

            CommandBuffer.DestroyEntity(entity);
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        //Instead of performing structural changes directly, a Job can add a command to an EntityCommandBuffer to perform such changes on the main thread after the Job has finished.
        //Command buffers allow you to perform any, potentially costly, calculations on a worker thread, while queuing up the actual insertions and deletions for later.

        // Schedule the job that will add Instantiate commands to the EntityCommandBuffer.
        var job = new TileSpawnJob
        {
            CommandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer()
        }.ScheduleSingle(this, inputDeps);


        // SpawnJob runs in parallel with no sync point until the barrier system executes.
        // When the barrier system executes we want to complete the SpawnJob and then play back the commands (Creating the entities and placing them).
        // We need to tell the barrier system which job it needs to complete before it can play back the commands.
        m_EntityCommandBufferSystem.AddJobHandleForProducer(job);

        return job;
    }


}
