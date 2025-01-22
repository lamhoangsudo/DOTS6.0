using Unity.Entities;
using Unity.VisualScripting;

public struct Bullet : IComponentData
{
    public float damage;
    public float speed;
}
