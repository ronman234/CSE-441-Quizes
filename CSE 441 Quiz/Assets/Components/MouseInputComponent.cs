//using System; 
using Unity.Entities;
using Unity.Mathematics;

//[Serializable]
public struct MouseInput : IComponentData
{
    public bool leftClick;
    public bool rightClick;
    public float3 MousePosition;
}

[UnityEngine.DisallowMultipleComponent]
public class MouseInputComponent : ComponentDataProxy<MouseInput> { }
