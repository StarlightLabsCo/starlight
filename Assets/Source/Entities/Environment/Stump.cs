public class Stump : ChoppableEntity
{
    public Stump(string id, string name) : base(id, name)
    {
    }

    public override Item itemOnDeath { get => new Wood(); set => base.itemOnDeath = value; }
}