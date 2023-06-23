using UnityEngine;

public class CoalOreMedium : MineableEntity
{
    public CoalOreMedium(string id, string name) : base(id, name)
    {
    }

    protected override void Awake()
    {
        base.Awake();

        Id = "coal_ore_medium_" + System.Guid.NewGuid().ToString();
        Name = "Coal Ore Deposit (Medium)";
    }

    public override Item itemOnDeath { get => new Coal(); set => base.itemOnDeath = value; }
    public override GameObject entityOnDeath { get => Resources.Load("CoalOreSmall") as GameObject; set => base.entityOnDeath = value; }
}