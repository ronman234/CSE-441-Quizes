using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public struct UnitSpawning : IComponentData
{
    public Entity Prefab;
}
