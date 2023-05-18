using UnityEngine;

public class CopperOreLarge : MineableEntity
{
    public CopperOreLarge(string id, string name) : base(id, name)
    {
    }

    public override Item itemOnDeath { get => new Copper(); set => base.itemOnDeath = value; }
    public override GameObject entityOnDeath { get => Resources.Load("CopperOreMedium") as GameObject; set => base.entityOnDeath = value; }
}