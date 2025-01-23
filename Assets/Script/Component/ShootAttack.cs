using Unity.Entities;
using Unity.Mathematics;

public struct ShootAttack : IComponentData
{
    public float timer;
    public float timerMax;
    public float damage;
    public float attackDistance;
    public float3 bulletSpawnPosition;
}
