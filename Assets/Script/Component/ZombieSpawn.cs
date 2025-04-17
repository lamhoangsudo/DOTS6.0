using Unity.Entities;

public struct ZombieSpawn : IComponentData
{
    public float timer;
    public float timerMax;
    public float zombieRandomWalkingDistanceMin;
    public float zombieRandomWalkingDistanceMax;
    public int nearbyZombieCountMax;
    public float nearbyZombieDistance;
}
