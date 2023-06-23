public class Stump : ChoppableEntity
{
    public Stump(string id, string name) : base(id, name)
    {
    }

    protected override void Awake()
    {
        base.Awake();

        Id = "tree_stump_" + System.Guid.NewGuid().ToString();
        Name = "Tree Stump";
    }

    public override Item itemOnDeath { get => new Wood(); set => base.itemOnDeath = value; }
}