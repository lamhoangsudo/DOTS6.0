using Unity.Entities;

public struct ShootAttack : IComponentData
{
    public float timer;
    public float timerMax;
}
