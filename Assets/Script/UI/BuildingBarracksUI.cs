using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

public class BuildingBarracksUI : MonoBehaviour
{
    [SerializeField] private Button scoutBtn, soldierBtn;
    [SerializeField] private Image progressBar;
    [SerializeField] private RectTransform container;
    [SerializeField] private Transform template;
    private Entity buildingBarracksEntity;
    private EntityManager entityManager;
    private DynamicBuffer<SpawnUnitType> spawnUnitTypes;
    private void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        scoutBtn.onClick.AddListener(() =>
        {
            UnitTypeSO unitTypeSO = GameAssets.instance.unitTypeListSO.GetUnitTypeSO(UnitTypeSO.UnitType.Scout);
            if(!ResourceManager.Instance.HasEnoughResource(unitTypeSO.resourceAmounts)) return;
            ResourceManager.Instance.SpendResourceAmount(unitTypeSO.resourceAmounts);
            spawnUnitTypes = entityManager.GetBuffer<SpawnUnitType>(buildingBarracksEntity, false);
            entityManager.SetComponentData(buildingBarracksEntity, new BuildingBarracksUnitEnqueue
            {
                unitType = UnitTypeSO.UnitType.Scout,
            });
            entityManager.SetComponentEnabled<BuildingBarracksUnitEnqueue>(buildingBarracksEntity, true);
        });
        soldierBtn.onClick.AddListener(() =>
        {
            UnitTypeSO unitTypeSO = GameAssets.instance.unitTypeListSO.GetUnitTypeSO(UnitTypeSO.UnitType.Soldier);
            if (!ResourceManager.Instance.HasEnoughResource(unitTypeSO.resourceAmounts)) return;
            ResourceManager.Instance.SpendResourceAmount(unitTypeSO.resourceAmounts);
            spawnUnitTypes = entityManager.GetBuffer<SpawnUnitType>(buildingBarracksEntity, false);
            entityManager.SetComponentData(buildingBarracksEntity, new BuildingBarracksUnitEnqueue
            {
                unitType = UnitTypeSO.UnitType.Soldier,
            });
            entityManager.SetComponentEnabled<BuildingBarracksUnitEnqueue>(buildingBarracksEntity, true);
        });
        UnitSelectionManager.Instance.OnSelectChange += Instance_OnSelectChange;
        DOTSEventsManager.Instance.OnBarracksUnitQueueChanged += Instance_OnBarracksUnitQueueChanged;
        Hide();
    }

    private void Instance_OnBarracksUnitQueueChanged(object sender, System.EventArgs e)
    {
        if (buildingBarracksEntity != (Entity)sender) return;
        UpdateUnitQueueVisual();
    }

    private void Update()
    {
        UpdateProgressBarVisual();
    }
    private void UpdateProgressBarVisual()
    {
        if (buildingBarracksEntity != Entity.Null)
        {
            BuildingBarracks buildingBarracks = entityManager.GetComponentData<BuildingBarracks>(buildingBarracksEntity);
            if (buildingBarracks.activeUnitType != UnitTypeSO.UnitType.None)
            {
                progressBar.fillAmount = buildingBarracks.progress / buildingBarracks.maxProgress;
            }
            else
            {
                progressBar.fillAmount = 0;
                return;
            }
        }
        else
        {
            progressBar.fillAmount = 0;
            return;
        }
    }
    private void Instance_OnSelectChange(object sender, System.EventArgs e)
    {
        EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<Select, BuildingBarracks>().Build(entityManager);
        NativeArray<Entity> entities = entityQuery.ToEntityArray(Allocator.Temp);
        if (entities.Length > 0)
        {
            buildingBarracksEntity = entities[0];
            Show();
            UpdateProgressBarVisual();
            UpdateUnitQueueVisual();
        }
        else
        {
            buildingBarracksEntity = Entity.Null;
            Hide();
        }
    }
    private void UpdateUnitQueueVisual()
    {
        foreach (Transform child in container)
        {
            if (child == template) continue;
            Destroy(child.gameObject);
        }
        spawnUnitTypes = entityManager.GetBuffer<SpawnUnitType>(buildingBarracksEntity, true);
        foreach (SpawnUnitType spawnUnitType in spawnUnitTypes)
        {
            RectTransform rect = Instantiate(template, container).GetComponent<RectTransform>();
            rect.gameObject.SetActive(true);
            UnitTypeSO unitTypeSO = GameAssets.instance.unitTypeListSO.GetUnitTypeSO(spawnUnitType.unitType);
            rect.GetComponent<Image>().sprite = unitTypeSO.sprite;
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
