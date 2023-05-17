using UnityEngine;

public class CopperOre : MineableEntity
{
    public CopperOre(string id, string name) : base(id, name)
    {
    }

    public override Item itemOnDeath { get => new Copper(); set => base.itemOnDeath = value; }
}