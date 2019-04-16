using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public struct GridSpawning : IComponentData
{
    public int CountX;
    public int CountZ;

    public Entity Prefab;
}
