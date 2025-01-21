using Unity.Entities;

public struct Select : IComponentData, IEnableableComponent
{
    public Entity visualEntity;
    public float scale;
    //event cant use in dots
    //using data
    public bool OnSelect;
    public bool OnDeSelect;
    public void SetSelect(bool OnSelect)
    {
        this.OnSelect = OnSelect;
        OnDeSelect = !this.OnSelect;
        return;
    }
}
