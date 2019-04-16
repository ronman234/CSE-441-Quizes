using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using System;

[Serializable]
public struct Health : IComponentData
{
    public float currentHealth;
}

public class HealthComponent : ComponentDataProxy<Health> { }
