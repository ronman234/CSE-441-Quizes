using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct GridData : IComponentData
{
    public int flowData;
    
}


public class GridDataComponent : ComponentDataProxy<GridData> { }
