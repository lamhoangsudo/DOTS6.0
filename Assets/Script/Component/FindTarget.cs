using Unity.Entities;

public struct FindTarget : IComponentData
{
    public float findingRange;
    public Faction targetFaction;
    public float timer;
    public float timerMax;
}
