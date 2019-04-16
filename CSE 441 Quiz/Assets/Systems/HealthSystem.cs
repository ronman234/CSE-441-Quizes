using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Jobs;
using System;
using Unity.Collections;

public class HealthBarrier : EntityCommandBufferSystem { }

[UpdateAfter(typeof(DamageSystem))]
public class HealthSystem : JobComponentSystem
{

    private HealthBarrier m_Barrier;

    protected override void OnCreateManager()
    {
        m_Barrier = World.GetExistingSystem<HealthBarrier>();
    }

    [BurstCompile]
    public struct HealthJob : IJobForEachWithEntity<Health>
    {
        public EntityCommandBuffer.Concurrent commandBuffer;

        public void Execute(Entity entity, int i, ref Health health)
        {
            if(health.currentHealth == 0)
            {
                commandBuffer.DestroyEntity(i, entity);
            }
        }
    }


    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
            var job = new HealthJob
            {
                commandBuffer = m_Barrier.CreateCommandBuffer().ToConcurrent()
            };

            var jobHandle = job.Schedule(this, inputDeps);
            jobHandle.Complete();

            return jobHandle;

    }
}
