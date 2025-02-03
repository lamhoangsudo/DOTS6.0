using Unity.Entities;
using Unity.Mathematics;

public struct ShootAttack : IComponentData
{
    public float timer;
    public float timerMax;
    public float damage;
    public float attackDistance;
    public float3 bulletSpawnPosition;
    public OnShootEvent OnShoot;
    public struct OnShootEvent
    {
        public float3 position;
        public bool trigger;
    }
}
