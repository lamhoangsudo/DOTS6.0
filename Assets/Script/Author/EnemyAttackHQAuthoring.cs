using Unity.Entities;
using UnityEngine;

public class EnemyAttackHQAuthoring : MonoBehaviour
{
    public class EnemyAttackHQAuthoringBaker : Baker<EnemyAttackHQAuthoring>
    {
        public override void Bake(EnemyAttackHQAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new EnemyAttackHQ
            {

            });
        }
    }
}
public struct EnemyAttackHQ : IComponentData
{

}


