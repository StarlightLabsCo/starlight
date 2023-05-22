using UnityEngine;

public class IronOreLarge : MineableEntity
{
    public IronOreLarge(string id, string name) : base(id, name)
    {
    }

    public void Start()
    {
        Id = "iron_ore_large";
        Name = "Iron Ore Deposit (Large)";
    }


    public override Item itemOnDeath { get => new Iron(); set => base.itemOnDeath = value; }
    public override GameObject entityOnDeath { get => Resources.Load("IronOreMedium") as GameObject; set => base.entityOnDeath = value; }
}