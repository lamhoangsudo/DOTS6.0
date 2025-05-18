using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class RagdollManager : MonoBehaviour
{
    [SerializeField] private UnitTypeListSO listSO;
    private void Start()
    {
        DOTSEventsManager.Instance.OnUnitDead += Instance_OnUnitDead; ;
    }

    private void Instance_OnUnitDead(object sender, System.EventArgs e)
    {
        Entity entity = (Entity)sender;
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        if (entityManager.HasComponent<UnitTypeSOHolder>(entity))
        {
            LocalTransform localTransform = entityManager.GetComponentData<LocalTransform>(entity);
            UnitTypeSOHolder unitTypeSOHolder = entityManager.GetComponentData<UnitTypeSOHolder>(entity);
            UnitTypeSO unitTypeSO = listSO.GetUnitTypeSO(unitTypeSOHolder.unitType);
            Instantiate(unitTypeSO.ragdollPrefab, localTransform.Position, Quaternion.identity);
        }
    }
}
