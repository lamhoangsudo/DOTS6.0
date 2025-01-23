using Unity.Entities;

public struct Health : IComponentData
{
    public float health;
    public float healthMax;
    public bool OnValueHealthChange;
}
