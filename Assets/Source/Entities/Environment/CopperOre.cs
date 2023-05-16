using UnityEngine;

public class CopperOre : Entity
{
    public CopperOre(string id, string name) : base(id, name)
    {
    }

    public override Item itemOnDeath { get => new Copper(); set => base.itemOnDeath = value; }
}