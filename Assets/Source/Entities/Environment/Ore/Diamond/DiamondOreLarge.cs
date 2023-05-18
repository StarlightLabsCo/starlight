using UnityEngine;

public class DiamondOreLarge : MineableEntity
{
    public DiamondOreLarge(string id, string name) : base(id, name)
    {
    }

    public override Item itemOnDeath { get => new Diamond(); set => base.itemOnDeath = value; }
    public override GameObject entityOnDeath { get => Resources.Load("DiamondOreMedium") as GameObject; set => base.entityOnDeath = value; }
}