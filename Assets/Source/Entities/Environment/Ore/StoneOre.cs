using UnityEngine;

public class StoneOre : MineableEntity
{
    public StoneOre(string id, string name) : base(id, name)
    {
    }

    public override Item itemOnDeath { get => new Stone(); set => base.itemOnDeath = value; }
}