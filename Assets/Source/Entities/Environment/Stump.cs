public class Stump : ChoppableEntity
{
    public Stump(string id, string name) : base(id, name)
    {
    }

    public void Start()
    {
        Id = "stump";
        Name = "Stump";
    }

    public override Item itemOnDeath { get => new Wood(); set => base.itemOnDeath = value; }
}