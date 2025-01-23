using Unity.Entities;

public struct ZombieSpawn : IComponentData
{
    public float timer;
    public float timerMax;
}
