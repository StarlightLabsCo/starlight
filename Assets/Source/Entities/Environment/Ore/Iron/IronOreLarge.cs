using UnityEngine;

public class IronOreLarge : MineableEntity
{
    public IronOreLarge(string id, string name) : base(id, name)
    {
    }

    protected override void Awake()
    {
        base.Awake();

        Id = "iron_ore_large_" + System.Guid.NewGuid().ToString();
        Name = "Iron Ore Deposit (Large)";
    }

    public override Item itemOnDeath { get => new Iron(); set => base.itemOnDeath = value; }
    public override GameObject entityOnDeath { get => Resources.Load("IronOreMedium") as GameObject; set => base.entityOnDeath = value; }
}