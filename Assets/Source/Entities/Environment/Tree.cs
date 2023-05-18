using UnityEngine;

public class Tree : ChoppableEntity
{
    public Tree(string id, string name) : base(id, name)
    {
    }

    public override Item itemOnDeath { get => new Wood(); set => base.itemOnDeath = value; }

    // Load stump prefab
    public override GameObject entityOnDeath { get => Resources.Load("Stump") as GameObject; set => base.entityOnDeath = value; }
}