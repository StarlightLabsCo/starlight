using UnityEngine;

public class IronOreMedium : MineableEntity
{
    public IronOreMedium(string id, string name) : base(id, name)
    {
    }

    protected override void Awake()
    {
        base.Awake();

        Id = "iron_ore_medium_" + System.Guid.NewGuid().ToString();
        Name = "Iron Ore Deposit (Medium)";
    }

    public override Item itemOnDeath { get => new Iron(); set => base.itemOnDeath = value; }
    public override GameObject entityOnDeath { get => Resources.Load("IronOreSmall") as GameObject; set => base.entityOnDeath = value; }
}