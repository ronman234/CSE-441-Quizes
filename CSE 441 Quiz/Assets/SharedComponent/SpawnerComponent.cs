using System;
using UnityEngine;
using Unity.Entities;

[Serializable]
public struct Spawner : ISharedComponentData
{
    public GameObject prefab;
}

public class SpawnerComponent : SharedComponentDataProxy<Spawner> { }
