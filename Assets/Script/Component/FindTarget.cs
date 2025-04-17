using Unity.Entities;

public struct FindTarget : IComponentData
{
    public float findingRange;
    public FactionType targetFaction;
    public float timer;
    public float timerMax;
}
