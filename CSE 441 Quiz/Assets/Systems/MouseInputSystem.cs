using System;
using Unity.Collections;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Jobs;
using Unity.Physics;

public class MouseInputSystem : JobComponentSystem
{

    [BurstCompile]
    struct MouseInputJob: IJobForEach<MouseInput>
    {
        public bool leftClick;
        public bool rightClick;
        public float3 mousePosition;

        public void Execute(ref MouseInput mi)
        {
            mi.leftClick = leftClick;
            mi.rightClick = rightClick;
            mi.MousePosition = mousePosition;
        }
    }

    [BurstCompile]
    public struct RaycastJob : IJobParallelFor
    {
        [ReadOnly] public Unity.Physics.CollisionWorld world;
        [ReadOnly] public NativeArray<Unity.Physics.RaycastInput> inputs;
        public NativeArray<Unity.Physics.RaycastHit> results;

        public void Execute(int index)
        {
            Unity.Physics.RaycastHit hit;
            world.CastRay(inputs[index], out hit);
            results[index] = hit;
        }
    }

    public static JobHandle ScheduleBatchRayCast(Unity.Physics.CollisionWorld world,
        NativeArray<Unity.Physics.RaycastInput> inputs, NativeArray<Unity.Physics.RaycastHit> results)
    {
        JobHandle rcj = new RaycastJob
        {
            inputs = inputs,
            results = results,
            world = world

        }.Schedule(inputs.Length, 5);
        return rcj;
    }

    public static void SingleRayCast(Unity.Physics.CollisionWorld world, Unity.Physics.RaycastInput input,
        ref Unity.Physics.RaycastHit result)
    {
        var rayCommands = new NativeArray<Unity.Physics.RaycastInput>(1, Allocator.TempJob);
        var rayResults = new NativeArray<Unity.Physics.RaycastHit>(1, Allocator.TempJob);
        rayCommands[0] = input;
        var handle = ScheduleBatchRayCast(world, rayCommands, rayResults);
        handle.Complete();
        result = rayResults[0];
        rayCommands.Dispose();
        rayResults.Dispose();
    }



    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var physicsWorldSystem = Unity.Entities.World.Active.GetExistingSystem<Unity.Physics.Systems.BuildPhysicsWorld>();
        var collisionWorld = physicsWorldSystem.PhysicsWorld.CollisionWorld;

        var mousePostion = Input.mousePosition;
        var unityRay = Camera.main.ScreenPointToRay(mousePostion);
        var ray = new Unity.Physics.Ray(unityRay.origin, unityRay.direction * 10000);

        Unity.Physics.RaycastInput input = new Unity.Physics.RaycastInput()
        {
            Ray = ray,
            Filter = new CollisionFilter()
            {
                CategoryBits = ~0u,
                MaskBits = ~0u,
                GroupIndex = 0
            }

        };

        Unity.Physics.RaycastHit hit = new Unity.Physics.RaycastHit();

        SingleRayCast(collisionWorld, input, ref hit);
        bool haveHit = collisionWorld.CastRay(input, out hit);
        if(haveHit)
        {
            mousePostion = new float3(hit.Position.x, hit.Position.y, hit.Position.z);
            
        }

        //Debug.Log("Mouse Positon: " + mousePostion);

        var job = new MouseInputJob
        {
            leftClick = Input.GetMouseButtonDown(0),
            rightClick = Input.GetMouseButtonDown(1),
            mousePosition = mousePostion
        };
        

        return job.Schedule(this, inputDeps);
    }
}
