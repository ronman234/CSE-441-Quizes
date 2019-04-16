using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Transforms;
using Unity.Collections;

public class UnitMovementSystem : JobComponentSystem
{
    [RequireComponentTag(typeof(Selected))]
    public struct UnitMoveJob : IJobForEach<MouseInput, Unit, Translation>
    {
        public void Execute( ref MouseInput mi, [ReadOnly] ref Unit u, ref Translation t )
        {
            if(mi.rightClick)
            {
                t.Value = mi.MousePosition;
            }
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new UnitMoveJob();

        return job.Schedule(this, inputDeps);
    }
}
