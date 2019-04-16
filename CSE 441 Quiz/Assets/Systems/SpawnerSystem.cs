using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Rendering;

public class SpawnerSystem : JobComponentSystem
{

    EndSimulationEntityCommandBufferSystem m_EntityCommandBufferSystem1;

    protected override void OnCreateManager()
    {
        // Cache the EndSimulationBarrier in a field, so we don't have to create it every frame
        m_EntityCommandBufferSystem1 = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    struct UnitSpawnJob : IJobForEachWithEntity<UnitSpawning, LocalToWorld, MouseInput>
    {
        public EntityCommandBuffer CommandBuffer;

        public void Execute(Entity entity, int index, [ReadOnly] ref UnitSpawning spawner,
            [ReadOnly] ref LocalToWorld location, [ReadOnly] ref MouseInput mi)
        {
            var instance = CommandBuffer.Instantiate(spawner.Prefab);
            CommandBuffer.SetComponent(instance, new Translation { Value = new float3(mi.MousePosition.x, 2, mi.MousePosition.z) });
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        //Instead of performing structural changes directly, a Job can add a command to an EntityCommandBuffer to perform such changes on the main thread after the Job has finished.
        //Command buffers allow you to perform any, potentially costly, calculations on a worker thread, while queuing up the actual insertions and deletions for later.

        if (Input.GetMouseButtonDown(1))
        {
            // Schedule the job that will add Instantiate commands to the EntityCommandBuffer.
            var job = new UnitSpawnJob
            {
                CommandBuffer = m_EntityCommandBufferSystem1.CreateCommandBuffer()
            }.ScheduleSingle(this, inputDeps);

            m_EntityCommandBufferSystem1.AddJobHandleForProducer(job);

            return job;
        }

        return inputDeps;
    }
}