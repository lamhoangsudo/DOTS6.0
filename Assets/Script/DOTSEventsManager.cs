using System;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.EventSystems;

public class DOTSEventsManager : MonoBehaviour
{
    public static DOTSEventsManager Instance { get; private set; }
    public event EventHandler OnBarracksUnitQueueChanged;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void TriggerOnBarracksUnitQueueChanged(NativeList<Entity> entities)
    {
        foreach(Entity entity in entities)
        {
            OnBarracksUnitQueueChanged?.Invoke(entity, new EventArgs());
        }
    }
}
