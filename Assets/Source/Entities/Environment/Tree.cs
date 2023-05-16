using UnityEngine;

public class Tree : Entity
{
    public Tree(string id, string name) : base(id, name)
    {
    }

    public override Item itemOnDeath { get => new Wood(); set => base.itemOnDeath = value; }
}