using UnityEngine;

public class Tree : ChoppableEntity
{
    public Tree(string id, string name) : base(id, name)
    {
    }

    protected override void Awake()
    {
        base.Awake();

        Id = "tree_" + System.Guid.NewGuid().ToString();
        Name = "Tree";
    }

    public override Item itemOnDeath { get => new Wood(); set => base.itemOnDeath = value; }

    // Load stump prefab
    public override GameObject entityOnDeath { get => Resources.Load("Stump") as GameObject; set => base.entityOnDeath = value; }
}