using UnityEngine;

public class CoalOre : MineableEntity
{
    public CoalOre(string id, string name) : base(id, name)
    {
    }

    public override Item itemOnDeath { get => new Coal(); set => base.itemOnDeath = value; }
}