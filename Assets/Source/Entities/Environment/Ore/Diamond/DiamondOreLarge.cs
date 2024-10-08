using UnityEngine;

public class DiamondOreLarge : MineableEntity
{
    public DiamondOreLarge(string id, string name) : base(id, name)
    {
    }

    protected override void Awake()
    {
        base.Awake();

        Id = "diamond_ore_large_" + System.Guid.NewGuid().ToString();
        Name = "Diamond Ore Deposit (Large)";
    }

    public override Item itemOnDeath { get => new Diamond(); set => base.itemOnDeath = value; }
    public override GameObject entityOnDeath { get => Resources.Load("DiamondOreMedium") as GameObject; set => base.entityOnDeath = value; }
}