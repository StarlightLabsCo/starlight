using UnityEngine;

public class CoalOreMedium : MineableEntity
{
    public CoalOreMedium(string id, string name) : base(id, name)
    {
    }

    public void Start()
    {
        Id = "coal_ore_medium";
        Name = "Coal Ore Deposit (Medium)";
    }

    public override Item itemOnDeath { get => new Coal(); set => base.itemOnDeath = value; }
    public override GameObject entityOnDeath { get => Resources.Load("CoalOreSmall") as GameObject; set => base.entityOnDeath = value; }
}