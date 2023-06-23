using UnityEngine;

public class CopperOreSmall : MineableEntity
{
    public CopperOreSmall(string id, string name) : base(id, name)
    {
    }

    protected override void Awake()
    {
        base.Awake();

        Id = "copper_ore_small_" + System.Guid.NewGuid().ToString();
        Name = "Copper Ore Deposit (Small)";
    }

    public override Item itemOnDeath { get => new Copper(); set => base.itemOnDeath = value; }
}