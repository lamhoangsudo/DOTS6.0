using Unity.Entities;
using UnityEngine.Rendering;

public struct ActiveAnimation : IComponentData
{
    public int frame;
    public int frameMax;
    public float frameTimer;
    public float frameTimerMax;
    public BatchMeshID frame0;
    public BatchMeshID frame1;
}
