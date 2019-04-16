using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Collections;
using Unity.Mathematics;

public class DamageSystem : JobComponentSystem
{

    public struct DamageJob : IJobForEachWithEntity<MouseInput, Health>
    {

        public void Execute(Entity entity, int index, [ReadOnly]ref MouseInput mI, ref Health health)
        {
            if (mI.leftClick)
            {
                health.currentHealth = 0;
            }
        }

    }


    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new DamageJob{};


        return job.Schedule(this, inputDeps);
    }
}
