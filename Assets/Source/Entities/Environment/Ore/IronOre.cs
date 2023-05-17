using UnityEngine;

public class IronOre : MineableEntity
{
    public IronOre(string id, string name) : base(id, name)
    {
    }

    public override Item itemOnDeath { get => new Iron(); set => base.itemOnDeath = value; }
}