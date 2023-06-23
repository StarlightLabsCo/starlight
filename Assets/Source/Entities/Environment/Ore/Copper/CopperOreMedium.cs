using UnityEngine;

public class CopperOreMedium : MineableEntity
{
    public CopperOreMedium(string id, string name) : base(id, name)
    {
    }


    protected override void Awake()
    {
        base.Awake();

        Id = "copper_ore_medium_" + System.Guid.NewGuid().ToString();
        Name = "Copper Ore Deposit (Medium)";
    }

    public override Item itemOnDeath { get => new Copper(); set => base.itemOnDeath = value; }
    public override GameObject entityOnDeath { get => Resources.Load("CopperOreSmall") as GameObject; set => base.entityOnDeath = value; }
}