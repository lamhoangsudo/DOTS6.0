using Unity.Entities;

public struct MeleeAttack : IComponentData
{
    public float attackDamage;
    public float timer;
    public float attackTimerMax;
    public float colliderSize;
    public bool onAttack;
}
