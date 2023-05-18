using UnityEngine;

public class CoalOreLarge : MineableEntity
{
    public CoalOreLarge(string id, string name) : base(id, name)
    {
    }

    public override Item itemOnDeath { get => new Coal(); set => base.itemOnDeath = value; }
    public override GameObject entityOnDeath { get => Resources.Load("CoalOreMedium") as GameObject; set => base.entityOnDeath = value; }
}